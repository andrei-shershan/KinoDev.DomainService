using System.Text.Json;
using KinoDev.DomainService.Infrastructure.ConfigurationModels;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.DtoModels.Orders;
using KinoDev.Shared.Services.Abstractions;
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
            _messageBrokerService.SubscribeAsync<OrderSummary>(
                _messageBrokerSettings.Queues.OrderFileCreated,
                async (orderSummary) =>
            {
                try
                {
                    await _orderProcessorService.ProcessOrderFileUrl(orderSummary);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing order file URL for order {OrderId}", orderSummary.Id);
                }
            });

            _messageBrokerService.SubscribeAsync<OrderSummary>(
                _messageBrokerSettings.Queues.EmailSent,
                async (orderSummary) =>
            {
                try
                {
                    await _orderProcessorService.ProcessOrderEmail(orderSummary);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing order email for order {OrderId}", orderSummary.Id);
                }
            });


            return Task.CompletedTask;
        }
    }
}