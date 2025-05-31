using KinoDev.Shared.DtoModels.Orders;

namespace KinoDev.DomainService.Infrastructure.Services.Abstractions
{
    public interface IOrderProcessorService
    {
        Task ProcessOrderFileUrl(OrderSummary orderSummary);

        Task ProcessOrderEmail(OrderSummary orderSummary);
    }
}