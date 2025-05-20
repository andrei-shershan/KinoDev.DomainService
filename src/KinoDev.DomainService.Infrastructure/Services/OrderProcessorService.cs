using KinoDev.DomainService.Infrastructure.Models;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public interface IOrderProcessorService
    {
        Task ProcessOrderFileUrl(OrderSummary orderSummary);

        Task ProcessOrderEmail(OrderSummary orderSummary);
    }

    public class OrderProcessorService : IOrderProcessorService
    {
        private readonly IMessageBrokerService _messageBrokerService;
        private readonly MessageBrokerSettings _messageBrokerSettings;
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderProcessorService> _logger;

        public OrderProcessorService(
            IMessageBrokerService messageBrokerService,
            IOptions<MessageBrokerSettings> messageBrokerSettings,
            IOrderService orderService,
            ILogger<OrderProcessorService> logger)
        {
            _messageBrokerService = messageBrokerService;
            _messageBrokerSettings = messageBrokerSettings.Value;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task ProcessOrderEmail(OrderSummary orderSummary)
        {
            await _orderService.SetEmailStatus(orderSummary.Id, true);
        }

        public async Task ProcessOrderFileUrl(OrderSummary orderSummary)
        {
            await _orderService.SetFileUrl(orderSummary.Id, orderSummary.FileUrl);

            // We need to set the file URL in the order summary
            orderSummary.FileUrl = orderSummary.FileUrl;

            _logger.LogInformation("Order file URL updated successfully for order ID: {OrderId}", orderSummary.Id);

            await _messageBrokerService.PublishAsync(
                orderSummary,
                _messageBrokerSettings.Topics.OrderFileUrlAdded
            );
        }
    }
}