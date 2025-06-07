using KinoDev.DomainService.Infrastructure.Services;
using KinoDev.DomainService.Infrastructure.Services.Abstractions;
using KinoDev.Shared.Services;
using KinoDev.Shared.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.DomainService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services, bool ignoreHostedService = false)
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
            services.AddTransient<IMessageBrokerService, RabbitMQService>();

            // Register the messaging subscriber as a hosted service if not ignored
            if (!ignoreHostedService)
            {
                services.AddHostedService<MessagingSubscriber>();
            }

            return services;
        }
    }
}
