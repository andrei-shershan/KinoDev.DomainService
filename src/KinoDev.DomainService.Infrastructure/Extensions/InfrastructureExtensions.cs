using KinoDev.DomainService.Infrastructure.Services;
using KinoDev.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.DomainService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IMovieService, MovieService>();

            services.AddTransient<IDateTimeService, DateTimeService>();

            services.AddTransient<IShowTimeService, ShowTimeService>();
            
            services.AddTransient<IOrderService, OrderService>();

            services.AddTransient<IMessageBrokerService, RabbitMQService>();

            services.AddHostedService<MessagingSubscriber>();

            return services;
        }
    }
}
