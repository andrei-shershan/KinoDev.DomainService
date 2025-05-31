using KinoDev.DomainService.Domain.Extensions;
using KinoDev.DomainService.Infrastructure.ConfigurationModels;
using KinoDev.DomainService.Infrastructure.Extensions;
using KinoDev.DomainService.WebApi.ConfigurationSettings;
using KinoDev.DomainService.WebApi.SetupExtensions;
using KinoDev.Shared.Models;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace KinoDev.DomainService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("Kinodev");
            var rabbitMq = builder.Configuration.GetSection("RabbitMQ");
            var messageBrokerSettings = builder.Configuration.GetSection("MessageBroker").Get<MessageBrokerSettings>();
            var domainDbSettings = builder.Configuration.GetSection("DomainDbSettings").Get<DomainDbSettings>();
            var ignoreHostedService = builder.Configuration.GetValue<bool>("IgnoreHostedService");
            var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();

            if (string.IsNullOrWhiteSpace(connectionString)
            || messageBrokerSettings == null
            || rabbitMq == null
            || domainDbSettings == null
            || authenticationSettings == null)
            {
                throw new InvalidConfigurationException("Unable to obtain required settings from configuration!");
            }

            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
            builder.Services.Configure<MessageBrokerSettings>(builder.Configuration.GetSection("MessageBroker"));

            builder.Services.InitializeDomain(connectionString, domainDbSettings.MigrationAssembly, domainDbSettings.LogSensitiveData);
            builder.Services.InitializeInfrastructure(ignoreHostedService);
            builder.Services.SetupAuthentication(authenticationSettings);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var disableHttpsRedirection = builder.Configuration.GetValue<bool>("DisableHttpsRedirection");
            if (!disableHttpsRedirection)
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
