using KinoDev.DomainService.Infrastructure.Services;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.Services;
using KinoDev.Shared.Services.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.DomainService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            bool ignoreHostedService = false
            )
        {
            // Infrastructure services
            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<IShowTimesService, ShowTimesService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IHallsService, HallsService>();
            services.AddTransient<ISlotService, SlotService>();
            services.AddTransient<IOrderProcessorService, OrderProcessorService>();

            // Helpers and utilities
            services.AddTransient<IDateTimeService, DateTimeService>();

            // Messaging services
            var messageBrokerName = configuration.GetValue<string>("MessageBrokerName");
            if (messageBrokerName == "RabbitMQ")
            {
                services.AddTransient<IMessageBrokerService, RabbitMQService>();
            }
            else if (messageBrokerName == "AzureServiceBus")
            {
                services.AddTransient<IMessageBrokerService, AzureServiceBusService>();
            }
            else
            {
                throw new InvalidOperationException("Invalid MessageBrokerName configuration value.");
            }

            // Register the messaging subscriber as a hosted service if not ignored
            if (!ignoreHostedService)
            {
                services.AddHostedService<MessagingSubscriber>();
            }

            var isInMemoryDbEnabled = configuration.GetValue<bool>("InMemoryDb:Enabled");
            if (isInMemoryDbEnabled)
            {
                services.AddTransient<ICacheRefreshService, CacheRefreshService>();
                services.AddHostedService<InitializerService>();
            }
            else
            {
                services.AddTransient<ICacheRefreshService, CacheRefreshEmptyService>();
            }

            var redisConn = configuration.GetValue<string>("Redis:ConnectionString");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConn;
            });

            return services;
        }
    }
}
