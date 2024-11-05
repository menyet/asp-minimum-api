using MasterData.Domain.Exceptions;

using Microsoft.OpenApi.Models;

namespace MasterData.Host.Configuration;

public static class Swagger
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration.GetSection("Authentication:Authority").Value ?? throw new ConfigurationErrorException("Missing Authentication:Authority");
        var authorizationUrl = $"{authority}/oauth2/v2.0/authorize";
        var tokenUrl = $"{authority}/oauth2/v2.0/token";
        var authScope = configuration.GetSection("Authentication:Scope").Value ?? throw new ConfigurationErrorException("Missing Authentication:Scope");

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("jwt", new OpenApiSecurityScheme
            {
                //In = ParameterLocation.Header,
                // Description = "Please enter token",
                // Name = "Authorization",
                Type = SecuritySchemeType.OAuth2,
                // BearerFormat = "JWT",
                // Scheme = "bearer"
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationUrl),
                        TokenUrl = new Uri(tokenUrl),
                        Scopes = new Dictionary<string, string> { { authScope, "MasterData API" } },
                    }
                }
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="jwt"
                        }
                    },
                    new[] { authScope }
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder EnableSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var clientId = configuration.GetSection("Authentication:ClientId").Value ?? throw new ConfigurationErrorException("Missing Authentication:ClientId");

        app.UseSwagger();
        
        app.UseSwaggerUI(options =>
        {

            options.OAuthUsePkce();

            options.OAuthClientId(clientId);

            options.EnableTryItOutByDefault();
        });

        return app;
    }
}
