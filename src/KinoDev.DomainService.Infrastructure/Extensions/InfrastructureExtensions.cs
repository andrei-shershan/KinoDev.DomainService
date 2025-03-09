using KinoDev.DomainService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.DomainService.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IMovieService, MovieService>();

            services.AddTransient<IDateTimeService, DateTimeService>();

            return services;
        }
    }
}
