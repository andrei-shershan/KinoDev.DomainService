using System.Text.Json;
using KinoDev.DomainService.Infrastructure.ConfigurationModels;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KinoDev.DomainService.Infrastructure.Services
{
    public class MessagingSubscriber : BackgroundService
    {
        private readonly IMessageBrokerService _messageBrokerService;
        private readonly MessageBrokerSettings _messageBrokerSettings;
        private readonly IOrderService _orderService;
        private readonly ILogger<MessagingSubscriber> _logger;

        private readonly IOrderProcessorService _orderProcessorService;

        public MessagingSubscriber(
            IMessageBrokerService messageBrokerService,
            IOptions<MessageBrokerSettings> messageBrokerSettings,
            IOrderService orderService,
            ILogger<MessagingSubscriber> logger,
            IOrderProcessorService orderProcessorService)
        {
            _messageBrokerService = messageBrokerService;
            _messageBrokerSettings = messageBrokerSettings.Value;
            _orderService = orderService;
            _logger = logger;
            _orderProcessorService = orderProcessorService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageBrokerService.SubscribeAsync(
                _messageBrokerSettings.Topics.OrderFileCreated,
                _messageBrokerSettings.Queues.OrderFileCreated,
                async (message) =>
            {
                try
                {
                    _logger.LogInformation("Received order completed message: {Message}", message);
                    var orderSummary = JsonSerializer.Deserialize<OrderSummary>(message);

                    await _orderProcessorService.ProcessOrderFileUrl(orderSummary);
                    _logger.LogInformation("Order file URL processed successfully for order ID: {OrderId}", orderSummary.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating order status for order");
                }
            });

            _messageBrokerService.SubscribeAsync(
                _messageBrokerSettings.Topics.EmailSent,
                _messageBrokerSettings.Queues.EmailSent,
                async (message) =>
            {
                try
                {
                    _logger.LogInformation("Received order completed message: {Message}", message);
                    var orderSummary = JsonSerializer.Deserialize<OrderSummary>(message);

                    await _orderProcessorService.ProcessOrderEmail(orderSummary);
                    _logger.LogInformation("Order email processed successfully for order ID: {OrderId}", orderSummary.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating order status for order");
                }
            });

            return Task.CompletedTask;
        }
    }
}