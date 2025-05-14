using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Helpers;
using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.Hall;
using KinoDev.Shared.DtoModels.Movies;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.DtoModels.ShowTimes;
using KinoDev.Shared.DtoModels.Tickets;
using KinoDev.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<OrderSummary> CreateOrderAsync(CreateOrderModel orderModel);

        Task<OrderSummary> GetOrderAsync(Guid id);

        Task<OrderDto> CompleteOrderAsync(Guid id);

        Task<bool> DeleteOrderAsync(Guid id);

        Task<OrderDto> UpdateOrderEmailAsync(Guid id, string email);

        Task<IEnumerable<OrderSummary>> GetCompletedOrdersAsync(IEnumerable<Guid> orderIds);

        Task<IEnumerable<OrderSummary>> GetCompletedOrdersByEmailAsync(string email);

        Task<bool> SetEmailStatus(Guid id, bool emailSent);
    }

    public class OrderService : IOrderService
    {
        private readonly KinoDevDbContext _dbContext;

        private readonly ILogger<OrderService> _logger;

        public OrderService(KinoDevDbContext dbContext, ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<OrderDto> CompleteOrderAsync(Guid id)
        {
            var dbOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (dbOrder == null || dbOrder.State != OrderState.New)
            {
                _logger.LogError($"Order with ID {id} not found or already completed.");
                return null;
            }

            dbOrder.State = OrderState.Completed;
            dbOrder.CompletedAt = DateTime.Now;

            var dbUpdateResult = _dbContext.Orders.Update(dbOrder);
            if (dbUpdateResult?.State != EntityState.Modified)
            {
                _logger.LogError($"Failed to update order with ID {id}.");
                return null;
            }

            await _dbContext.SaveChangesAsync();
            return new OrderDto()
            {
                Id = dbOrder.Id,
                CreatedAt = dbOrder.CreatedAt,
                CompletedAt = dbOrder.CompletedAt,
                Cost = dbOrder.Cost,
                State = dbOrder.State,
                Email = dbOrder.Email,
                EmailSent = dbOrder.EmailSent,
                UserId = dbOrder.UserId,
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
                _logger.LogError($"ShowTime with ID {orderModel.ShowTimeId} not found.");
                return null;
            }

            var seats = await _dbContext.Seats.Where(x => orderModel.SelectedSeatIds.Contains(x.Id)).ToListAsync();
            if (seats == null
                || seats.Count == 0
                || seats.Count != orderModel.SelectedSeatIds.Count())
            {
                _logger.LogError($"Seats not found for the provided IDs: {string.Join(", ", orderModel.SelectedSeatIds)}.");
                return null;
            }

            var orderCost = dbShowTime.Price * orderModel.SelectedSeatIds.Count();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var id = Guid.NewGuid();
                var order = new Order()
                {
                    Id = id,
                    CreatedAt = DateTime.Now,
                    Cost = orderCost,
                    State = OrderState.New,
                    Email = orderModel.Email,
                    EmailSent = false,
                    UserId = orderModel.UserId,
                };

                var dbAddOrderResult = await _dbContext.Orders.AddAsync(order);
                if (dbAddOrderResult?.State != EntityState.Added)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Failed to add order for ShowTime ID {orderModel.ShowTimeId}.");
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
                    CompletedAt = dbAddOrderResult.Entity.CompletedAt,
                    Email = dbAddOrderResult.Entity.Email,
                    EmailSent = dbAddOrderResult.Entity.EmailSent,
                    UserId = dbAddOrderResult.Entity.UserId,
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
                _logger.LogError($"Failed to create order for ShowTime ID {orderModel.ShowTimeId}: {ex.Message}");
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
                _logger.LogError($"Failed to delete order with ID {id}.");
                return false;
            }
        }

        public async Task<IEnumerable<OrderSummary>> GetCompletedOrdersAsync(IEnumerable<Guid> orderIds)
        {
            // TODO: Check SQL query performance
            // TODO: Opimise response
            var dbOrderData = await _dbContext.Orders
                .Join(_dbContext.Tickets, o => o.Id, t => t.OrderId, (o, t) => new { o, t })
                .Join(_dbContext.ShowTimes, x => x.t.ShowTimeId, st => st.Id, (x, st) => new { x.o, x.t, st })
                .Join(_dbContext.Seats, x => x.t.SeatId, s => s.Id, (x, s) => new { x.o, x.t, x.st, s })
                .Where(x => orderIds.Contains(x.o.Id) && x.o.State == OrderState.Completed)
                .ToListAsync();

            // TODO: Add validations
            if (dbOrderData == null || dbOrderData.Count == 0)
            {
                _logger.LogError($"No completed orders found for the provided IDs: {string.Join(", ", orderIds)}.");
                return null;
            }

            var result = new List<OrderSummary>();
            foreach (var order in dbOrderData.GroupBy(x => x.o.Id))
            {
                var dbShowTimeData = await _dbContext.ShowTimes
                    .Include(x => x.Movie)
                    .Include(x => x.Hall)
                    .FirstOrDefaultAsync(x => x.Id == order.FirstOrDefault().st.Id);

                if (dbShowTimeData == null || dbShowTimeData.Movie == null || dbShowTimeData.Hall == null)
                {
                    continue;
                }

                result.Add(new OrderSummary()
                {
                    CompletedAt = order.FirstOrDefault().o.CompletedAt,
                    CreatedAt = order.FirstOrDefault().o.CreatedAt,
                    Cost = order.FirstOrDefault().o.Cost,
                    Id = order.FirstOrDefault().o.Id,
                    State = order.FirstOrDefault().o.State,
                    Email = order.FirstOrDefault().o.Email,
                    EmailSent = order.FirstOrDefault().o.EmailSent,
                    UserId = order.FirstOrDefault().o.UserId,
                    ShowTimeSummary = new ShowTimeSummary()
                    {
                        Id = order.FirstOrDefault().st.Id,
                        Time = order.FirstOrDefault().st.Time,
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
                    Tickets = order.Select(x => new TickerSummary()
                    {
                        Number = x.s.Number,
                        Row = x.s.Row,
                        SeatId = x.s.Id,
                        Price = x.st.Price,
                        TicketId = x.t.Id,
                    }).ToList()
                });
            }

            return result;
        }

        public async Task<IEnumerable<OrderSummary>> GetCompletedOrdersByEmailAsync(string email)
        {
            var dbOrderData = await _dbContext.Orders
                .Join(_dbContext.Tickets, o => o.Id, t => t.OrderId, (o, t) => new { o, t })
                .Join(_dbContext.ShowTimes, x => x.t.ShowTimeId, st => st.Id, (x, st) => new { x.o, x.t, st })
                .Join(_dbContext.Seats, x => x.t.SeatId, s => s.Id, (x, s) => new { x.o, x.t, x.st, s })
                .Where(x => x.o.Email == email && x.o.State == OrderState.Completed)
                .ToListAsync();

            // TODO: Add validations
            if (dbOrderData == null || dbOrderData.Count == 0)
            {
                _logger.LogError($"No completed orders found for the provided email: {email}");
                return null;
            }

            var result = new List<OrderSummary>();
            foreach (var order in dbOrderData.GroupBy(x => x.o.Id))
            {
                var dbShowTimeData = await _dbContext.ShowTimes
                    .Include(x => x.Movie)
                    .Include(x => x.Hall)
                    .FirstOrDefaultAsync(x => x.Id == order.FirstOrDefault().st.Id);

                if (dbShowTimeData == null || dbShowTimeData.Movie == null || dbShowTimeData.Hall == null)
                {
                    continue;
                }

                result.Add(new OrderSummary()
                {
                    CompletedAt = order.FirstOrDefault().o.CompletedAt,
                    CreatedAt = order.FirstOrDefault().o.CreatedAt,
                    Cost = order.FirstOrDefault().o.Cost,
                    Id = order.FirstOrDefault().o.Id,
                    State = order.FirstOrDefault().o.State,
                    Email = order.FirstOrDefault().o.Email,
                    EmailSent = order.FirstOrDefault().o.EmailSent,
                    UserId = order.FirstOrDefault().o.UserId,
                    ShowTimeSummary = new ShowTimeSummary()
                    {
                        Id = order.FirstOrDefault().st.Id,
                        Time = order.FirstOrDefault().st.Time,
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
                    Tickets = order.Select(x => new TickerSummary()
                    {
                        Number = x.s.Number,
                        Row = x.s.Row,
                        SeatId = x.s.Id,
                        Price = x.st.Price,
                        TicketId = x.t.Id,
                    }).ToList()
                });
            }

            return result;
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
                _logger.LogError($"Order with ID {id} not found.");
                return null;
            }

            var dbShowTimeData = await _dbContext.ShowTimes
                .Include(x => x.Movie)
                .Include(x => x.Hall)
                .FirstOrDefaultAsync(x => x.Id == dbOrderData.FirstOrDefault().st.Id);

            if (dbShowTimeData == null || dbShowTimeData.Movie == null || dbShowTimeData.Hall == null)
            {
                _logger.LogError($"ShowTime with ID {dbOrderData.FirstOrDefault().st.Id} not found.");
                return null;
            }

            return new OrderSummary()
            {
                CompletedAt = dbOrderData.FirstOrDefault().o.CompletedAt,
                CreatedAt = dbOrderData.FirstOrDefault().o.CreatedAt,
                Cost = dbOrderData.FirstOrDefault().o.Cost,
                Id = dbOrderData.FirstOrDefault().o.Id,
                State = dbOrderData.FirstOrDefault().o.State,
                Email = dbOrderData.FirstOrDefault().o.Email,
                EmailSent = dbOrderData.FirstOrDefault().o.EmailSent,
                UserId = dbOrderData.FirstOrDefault().o.UserId,
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

        public async Task<bool> SetEmailStatus(Guid id, bool emailSent)
        {
            var dbOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (dbOrder == null)
            {
                _logger.LogError($"Order with ID {id} not found.");
                return false;
            }

            dbOrder.EmailSent = emailSent;
            var dbUpdateResult = _dbContext.Orders.Update(dbOrder);
            if (dbUpdateResult?.State != EntityState.Modified)
            {
                _logger.LogError($"Failed to update order with ID {id}.");
                return false;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<OrderDto> UpdateOrderEmailAsync(Guid id, string email)
        {
            var dbOrder = await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (dbOrder == null || dbOrder.State != OrderState.New)
            {
                _logger.LogError($"Order with ID {id} not found or already completed.");
                return null;
            }

            dbOrder.Email = email;

            var dbUpdateResult = _dbContext.Orders.Update(dbOrder);
            if (dbUpdateResult?.State != EntityState.Modified)
            {
                _logger.LogError($"Failed to update order with ID {id}.");
                return null;
            }

            await _dbContext.SaveChangesAsync();
            return new OrderDto()
            {
                Id = dbOrder.Id,
                CreatedAt = dbOrder.CreatedAt,
                CompletedAt = dbOrder.CompletedAt,
                Cost = dbOrder.Cost,
                State = dbOrder.State,
                Email = dbOrder.Email,
                EmailSent = dbOrder.EmailSent,
                UserId = dbOrder.UserId,
            };
        }
    }
}