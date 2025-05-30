using KinoDev.DomainService.Domain.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KinoDev.DomainService.Domain.Extensions
{
    public static class DomainExtentions
    {
        public static IServiceCollection InitializeDomain(
            this IServiceCollection services,
            string connectionString,
            string migrationAssembly
        )
        {
            services.AddDbContext<KinoDevDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    sql => sql.MigrationsAssembly(migrationAssembly)
                )
                // TODO: Allow it for local development only
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Debug);
            });

            return services;
        }
    }
}
