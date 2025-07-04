using System.Text;
using KinoDev.DomainService.Infrastructure.ConfigurationModels;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.Services.Abstractions;
using Microsoft.Extensions.Options;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class OrderProcessorService : IOrderProcessorService
    {
        private readonly IMessageBrokerService _messageBrokerService;
        private readonly MessageBrokerSettings _messageBrokerSettings;
        private readonly IOrderService _orderService;

        public OrderProcessorService(
            IMessageBrokerService messageBrokerService,
            IOptions<MessageBrokerSettings> messageBrokerSettings,
            IOrderService orderService)
        {
            _messageBrokerService = messageBrokerService;
            _messageBrokerSettings = messageBrokerSettings.Value;
            _orderService = orderService;
        }

        public async Task ProcessOrderEmail(OrderSummary orderSummary)
        {
            await _orderService.SetEmailStatus(orderSummary.Id, true);
        }

        public async Task ProcessOrderFileUrl(OrderSummary orderSummary)
        {
            var result = await _orderService.SetFileUrl(orderSummary.Id, orderSummary.FileUrl);
            if (result)
            {
                // We need to set the file URL in the order summary
                orderSummary.FileUrl = orderSummary.FileUrl;

                await _messageBrokerService.SendMessageAsync
                (
                    _messageBrokerSettings.Queues.OrderFileUrlAdded,
                    orderSummary
                );
            }
            else
            {
                // Log or handle the case where the file URL could not be set
                throw new InvalidOperationException($"Failed to set file URL for order {orderSummary.Id}");
            }
        }
    }
}