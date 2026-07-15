using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace MySolution.Infrastructure.Authentication;

/// <summary>
/// Provides extension methods for configuring JWT authentication in the application.
/// </summary>
public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
   
        // Binds the JWT settings from the configuration.
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration
                              .GetSection(JwtSettings.SectionName)
                              .Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JWT configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is missing.");

        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;

                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Check the Issuer
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    // Check the Audience
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    // Check the expiration time
                    ValidateLifetime = true,
                    // Check the signing key
                    ValidateIssuerSigningKey = true,
                    // Set the signing key
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }
}