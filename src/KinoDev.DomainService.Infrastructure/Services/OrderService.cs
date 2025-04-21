using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.DtoModels.ShowTimes;
using KinoDev.Shared.DtoModels.Tickets;
using KinoDev.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<OrderSummary> CreateOrderAsync(CreateOrderModel orderModel);
        Task<OrderSummary> GetOrderAsync(Guid id);

        Task<OrderDto> CompleteOrderAsync(Guid id);

        Task<bool> DeleteOrderAsync(Guid id);
    }

    public class OrderService : IOrderService
    {
        private readonly KinoDevDbContext _dbContext;

        public OrderService(KinoDevDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderDto> CompleteOrderAsync(Guid id)
        {
            var dbOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (dbOrder == null || dbOrder.State != OrderState.New)
            {
                return null;
            }

            dbOrder.State = OrderState.Completed;
            dbOrder.CompletedAt = DateTime.Now;

            var dbUpdateResult = _dbContext.Orders.Update(dbOrder);
            if (dbUpdateResult?.State != EntityState.Modified)
            {
                return null;
            }

            await _dbContext.SaveChangesAsync();
            return new OrderDto()
            {
                Id = dbOrder.Id,
                CreatedAt = dbOrder.CreatedAt,
                CompletedAt = dbOrder.CompletedAt,
                Cost = dbOrder.Cost,
                State = dbOrder.State                
            };
        }

        public async Task<OrderSummary> CreateOrderAsync(CreateOrderModel orderModel)
        {
            var dbShowTime = await _dbContext.ShowTimes
                .Include(x => x.Movie)
                .Include(x => x.Hall)
                .FirstOrDefaultAsync(x => x.Id == orderModel.ShowTimeId);

            if (dbShowTime == null)
            {
                return null;
            }

            var seats = await _dbContext.Seats.Where(x => orderModel.SelectedSeatIds.Contains(x.Id)).ToListAsync();
            if (seats == null
                || seats.Count == 0
                || seats.Count != orderModel.SelectedSeatIds.Count())
            {
                return null;
            }

            var orderCost = dbShowTime.Price * orderModel.SelectedSeatIds.Count();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var order = new Order()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Cost = orderCost,
                    State = OrderState.New
                };

                var dbAddOrderResult = await _dbContext.Orders.AddAsync(order);
                if (dbAddOrderResult?.State != EntityState.Added)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await _dbContext.SaveChangesAsync();

                var dbTickets = new List<Ticket>();
                foreach (var seatId in orderModel.SelectedSeatIds)
                {
                    var dbTicket = new Ticket()
                    {
                        Id = Guid.NewGuid(),
                        ShowTimeId = orderModel.ShowTimeId,
                        SeatId = seatId,
                        OrderId = dbAddOrderResult.Entity.Id
                    };

                    dbTickets.Add(dbTicket);
                }

                await _dbContext.Tickets.AddRangeAsync(dbTickets);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new OrderSummary()
                {
                    Id = dbAddOrderResult.Entity.Id,
                    Cost = dbAddOrderResult.Entity.Cost,
                    State = dbAddOrderResult.Entity.State,
                    CreatedAt = dbAddOrderResult.Entity.CreatedAt,
                    ShowTimeSummary = new ShowTimeSummary()
                    {
                        Id = dbShowTime.Id,
                        Time = dbShowTime.Time,
                        Movie = new MovieDto()
                        {
                            Id = dbShowTime.Movie.Id,
                            Name = dbShowTime.Movie.Name,
                            Description = dbShowTime.Movie.Description,
                            Duration = dbShowTime.Movie.Duration,
                            ReleaseDate = dbShowTime.Movie.ReleaseDate,
                            Url = dbShowTime.Movie.Url
                        },
                        Hall = new HallDto()
                        {
                            Id = dbShowTime.Hall.Id,
                            Name = dbShowTime.Hall.Name,
                        }
                    },
                    Tickets = dbTickets.Select(x => new TickerSummary()
                    {
                        Number = x.Seat.Number,
                        Row = x.Seat.Row,
                        SeatId = x.Seat.Id,
                        Price = dbShowTime.Price,
                        TicketId = x.Id,
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null || order.State != OrderState.New)
            {
                // if order is not found or already completed, nothing to delete
                return true;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Delete related tickets first
                var tickets = await _dbContext.Tickets.Where(t => t.OrderId == id).ToListAsync();
                _dbContext.Tickets.RemoveRange(tickets);

                // Delete the order
                _dbContext.Orders.Remove(order);
                
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<OrderSummary> GetOrderAsync(Guid id)
        {
            // TODO: Check SQL query performance
            // TODO: Opimise response
            var dbOrderData = await _dbContext.Orders
                .Join(_dbContext.Tickets, o => o.Id, t => t.OrderId, (o, t) => new { o, t })
                .Join(_dbContext.ShowTimes, x => x.t.ShowTimeId, st => st.Id, (x, st) => new { x.o, x.t, st })
                .Join(_dbContext.Seats, x => x.t.SeatId, s => s.Id, (x, s) => new { x.o, x.t, x.st, s })
                .Where(x => x.o.Id == id)
                .ToListAsync();

            // TODO: Add validations
            if (dbOrderData == null || dbOrderData.Count == 0 || dbOrderData.FirstOrDefault() == null)
            {
                return null;
            }

            var dbShowTimeData = await _dbContext.ShowTimes
                .Include(x => x.Movie)
                .Include(x => x.Hall)
                .FirstOrDefaultAsync(x => x.Id == dbOrderData.FirstOrDefault().st.Id);

            if (dbShowTimeData == null || dbShowTimeData.Movie == null || dbShowTimeData.Hall == null)
            {
                return null;
            }

            return new OrderSummary()
            {
                CompletedAt = dbOrderData.FirstOrDefault().o.CompletedAt,
                CreatedAt = dbOrderData.FirstOrDefault().o.CreatedAt,
                Cost = dbOrderData.FirstOrDefault().o.Cost,
                Id = dbOrderData.FirstOrDefault().o.Id,
                State = dbOrderData.FirstOrDefault().o.State,
                ShowTimeSummary = new ShowTimeSummary()
                {
                    Id = dbOrderData.FirstOrDefault().st.Id,
                    Time = dbOrderData.FirstOrDefault().st.Time,
                    Movie = new MovieDto()
                    {
                        Id = dbShowTimeData.Movie.Id,
                        Name = dbShowTimeData.Movie.Name,
                        Description = dbShowTimeData.Movie.Description,
                        Duration = dbShowTimeData.Movie.Duration,
                        ReleaseDate = dbShowTimeData.Movie.ReleaseDate,
                        Url = dbShowTimeData.Movie.Url
                    },
                    Hall = new HallDto()
                    {
                        Id = dbShowTimeData.Hall.Id,
                        Name = dbShowTimeData.Hall.Name,
                    }
                },
                Tickets = dbOrderData.Select(x => new TickerSummary()
                {
                    Number = x.s.Number,
                    Row = x.s.Row,
                    SeatId = x.s.Id,
                    Price = x.st.Price,
                    TicketId = x.t.Id,
                }).ToList()
            };
        }
    }
}