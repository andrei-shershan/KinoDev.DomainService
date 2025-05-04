using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace KinoDev.DomainService.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static IServiceCollection AddSerilogConsoleLogger(this IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(Log.Logger, dispose: true));

        return services;
    }
}