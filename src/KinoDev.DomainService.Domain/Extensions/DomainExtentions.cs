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
            string migrationAssembly,
            bool logSensitiveData = false
        )
        {
            services.AddDbContext<KinoDevDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    sql => sql.MigrationsAssembly(migrationAssembly)
                )
                .EnableSensitiveDataLogging(logSensitiveData)
                .LogTo(Console.WriteLine, LogLevel.None);
            });

            return services;
        }
    }
}
