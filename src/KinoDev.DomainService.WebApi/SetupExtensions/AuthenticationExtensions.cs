using KinoDev.DomainService.WebApi.ConfigurationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KinoDev.DomainService.WebApi.SetupExtensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection SetupAuthentication(
            this IServiceCollection services,
            AuthenticationSettings authenticationSettings
            )
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authenticationSettings.Issuer,
                        ValidAudience = authenticationSettings.Audiences.Internal,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.RequireHttpsMetadata = true; // Set to true in production

                    // Add this for Azure deployment
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Log the error for debugging
                            Console.WriteLine($"Authentication failed: {context.Exception}");
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
