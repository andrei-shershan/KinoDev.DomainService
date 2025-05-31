using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.Orders;

namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
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

        Task<bool> SetFileUrl(Guid id, string fileUrl);
    }
}