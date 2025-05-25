using KinoDev.DomainService.Infrastructure.Services;
using KinoDev.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.DomainService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services, bool ignoreHostedService = false)
        {
            services.AddTransient<IMovieService, MovieService>();

            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddTransient<IShowTimeService, ShowTimeService>();

            services.AddTransient<IOrderService, OrderService>();

            services.AddTransient<IHallsService, HallsService>();

            services.AddTransient<IMessageBrokerService, RabbitMQService>();

            if (!ignoreHostedService)
            {
                services.AddHostedService<MessagingSubscriber>();
            }

            services.AddTransient<IOrderProcessorService, OrderProcessorService>();

            return services;
        }
    }
}
