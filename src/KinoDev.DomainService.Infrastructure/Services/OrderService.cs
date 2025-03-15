using KinoDev.DomainService.Domain.Context;
using KinoDev.DomainService.Domain.DomainsModels;
using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.DtoModels.Tickets;
using KinoDev.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderModel orderModel);
    }

    public class OrderService : IOrderService
    {
        private readonly KinoDevDbContext _dbContext;

        public OrderService(KinoDevDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderModel orderModel)
        {
            var dbShowTime = await _dbContext.ShowTimes.FirstOrDefaultAsync(x => x.Id == orderModel.ShowTimeId);
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
                        ShowTimeId = orderModel.ShowTimeId,
                        SeatId = seatId,
                        OrderId = dbAddOrderResult.Entity.Id
                    };

                    dbTickets.Add(dbTicket);
                }

                await _dbContext.Tickets.AddRangeAsync(dbTickets);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new OrderDto()
                {
                    Id = dbAddOrderResult.Entity.Id,
                    Cost = dbAddOrderResult.Entity.Cost,
                    State = dbAddOrderResult.Entity.State,
                    CreatedAt = dbAddOrderResult.Entity.CreatedAt,
                    Ticket = dbTickets.Select(x => new TicketDto()
                    {
                        Id = x.Id,
                        ShowTimeId = x.ShowTimeId,
                        SeatId = x.SeatId,
                        OrderId = x.OrderId
                    }).ToList()
                };
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
    }
}