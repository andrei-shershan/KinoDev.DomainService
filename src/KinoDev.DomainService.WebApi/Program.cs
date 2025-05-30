
using KinoDev.DomainService.Domain.Extensions;
using KinoDev.DomainService.Infrastructure.Extensions;
using KinoDev.DomainService.Infrastructure.Models;
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
                //.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // TODO: move to settings
            var connectionString = builder.Configuration.GetConnectionString("Kinodev");
            var rabbitMq = builder.Configuration.GetSection("RabbitMQ");
            var messageBrokerSettings = builder.Configuration.GetSection("MessageBroker").Get<MessageBrokerSettings>();
            if (string.IsNullOrWhiteSpace(connectionString)
            || messageBrokerSettings == null
            || rabbitMq == null)
            {
                throw new InvalidConfigurationException("Unable to obtain settings from config!");
            }

            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
            builder.Services.Configure<MessageBrokerSettings>(builder.Configuration.GetSection("MessageBroker"));

            var migrationAssembly = "KinoDev.DomainService.WebApi";

            builder.Services.InitializeDomain(connectionString, migrationAssembly);

            var ignoreHostedService = builder.Configuration.GetValue<bool>("IgnoreHostedService");
            builder.Services.InitializeInfrastructure(ignoreHostedService);

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    // .AllowCredentials();
                });
            });

            var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
            if (authenticationSettings == null)
            {
                throw new InvalidConfigurationException("Unable to obtain AuthenticationSettings from configuration!");
            }

            builder.Services.SetupAuthentication(authenticationSettings);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // TODO: Disable for localhost only
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(); // Ensure CORS middleware is used

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
