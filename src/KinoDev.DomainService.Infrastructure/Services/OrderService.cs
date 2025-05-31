using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Mappers;
using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
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
            return dbOrder.ToDto();
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

                return GetOrderSummary(dbAddOrderResult.Entity, dbShowTime, dbTickets);
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
            foreach (var orderGroup in dbOrderData.GroupBy(x => x.o.Id))
            {
                var dbShowTimeData = await _dbContext.ShowTimes
                    .Include(x => x.Movie)
                    .Include(x => x.Hall)
                    .FirstOrDefaultAsync(x => x.Id == orderGroup.FirstOrDefault().st.Id);

                if (dbShowTimeData == null || dbShowTimeData.Movie == null || dbShowTimeData.Hall == null)
                {
                    continue;
                }

                var orderData = orderGroup.FirstOrDefault();
                if (orderData == null)
                {
                    _logger.LogError($"Order with ID {orderGroup.Key} not found.");
                    continue;
                }

                var orderSummary = GetOrderSummary(orderData.o, dbShowTimeData, orderGroup.Select(x => x.t));

                result.Add(orderSummary);
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

                var orderSummary = GetOrderSummary(order.FirstOrDefault().o, dbShowTimeData, order.Select(x => x.t));

                result.Add(orderSummary);
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

            return GetOrderSummary(dbOrderData.FirstOrDefault().o, dbShowTimeData, dbOrderData.Select(x => x.t));
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

        public Task<bool> SetFileUrl(Guid id, string fileUrl)
        {
            var dbOrder = _dbContext.Orders.FirstOrDefault(x => x.Id == id);
            if (dbOrder == null)
            {
                _logger.LogError($"Order with ID {id} not found.");
                return Task.FromResult(false);
            }

            dbOrder.FileUrl = fileUrl;
            var dbUpdateResult = _dbContext.Orders.Update(dbOrder);
            if (dbUpdateResult?.State != EntityState.Modified)
            {
                _logger.LogError($"Failed to update order with ID {id}.");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
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
            return dbOrder.ToDto();
        }

        private OrderSummary GetOrderSummary(Order order, ShowTime showTime, IEnumerable<Ticket?> tickets)
        {
            if (order == null || showTime == null || tickets == null)
            {
                _logger.LogError($"Order or ShowTime or Tickets data is null for Order ID {order?.Id}.");
                return null;
            }

            return new OrderSummary()
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                Cost = order.Cost,
                State = order.State,
                Email = order.Email,
                EmailSent = order.EmailSent,
                UserId = order.UserId,
                FileUrl = order.FileUrl,
                ShowTimeSummary = GetShowTimeSummary(showTime),
                Tickets = tickets.Select(ticket => GetTickerSummary(ticket, showTime)).ToList()
            };
        }

        private ShowTimeSummary GetShowTimeSummary(ShowTime showTime)
        {
            if (showTime == null)
            {
                return null;
            }

            return new ShowTimeSummary()
            {
                Id = showTime.Id,
                Time = showTime.Time,
                Movie = showTime.Movie.ToDto(),
                Hall = showTime.Hall.ToDto()
            };
        }

        private TickerSummary GetTickerSummary(Ticket ticket, ShowTime showTime)
        {
            if (ticket == null || showTime == null)
            {
                return null;
            }

            return new TickerSummary()
            {
                Number = ticket.Seat.Number,
                Row = ticket.Seat.Row,
                SeatId = ticket.Seat.Id,
                Price = showTime.Price,
                TicketId = ticket.Id
            };
        }
    }
}