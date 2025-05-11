using KinoDev.DomainService.Infrastructure.Models;
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

        public MessagingSubscriber(
            IMessageBrokerService messageBrokerService,
            IOptions<MessageBrokerSettings> messageBrokerSettings,
            IOrderService orderService,
            ILogger<MessagingSubscriber> logger)
        {
            _messageBrokerService = messageBrokerService;
            _messageBrokerSettings = messageBrokerSettings.Value;
            _orderService = orderService;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _messageBrokerService.SubscribeAsync(
                _messageBrokerSettings.Topics.EmailSent,
                _messageBrokerSettings.Queues.DomainServiceEmailSent,
                async (message) =>
            {
                _logger.LogInformation("Received message: {Message}", message);

                try
                {
                    // Clean up the message by removing quotes and whitespace
                    string cleanMessage = message.Trim().Trim('"');
                   
                    var orderId = Guid.Parse(cleanMessage);

                    await _orderService.SetEmailStatus(orderId, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating order status for order");
                }
            });
        }
    }
}