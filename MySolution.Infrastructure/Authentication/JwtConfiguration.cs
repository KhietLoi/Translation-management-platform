using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.Authentication;

/// <summary>
/// Provides extension methods for configuring JWT authentication in the application.
/// </summary>
public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
   
        // Binds the JWT settings from the configuration.
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var jwtSettings = configuration
                              .GetSection(JwtOptions.SectionName)
                              .Get<JwtOptions>()
                          ?? throw new InvalidOperationException("JWT configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is missing.");

        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var blacklistService = context.HttpContext
                            .RequestServices
                            .GetRequiredService<ITokenBlacklistService>();
                        
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        if (string.IsNullOrWhiteSpace(jti))
                        {
                            context.Fail("Missing JTI");
                            return;
                        }
                      
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var tokenSecurityStamp = context.Principal?.FindFirst("security_stamp")?.Value;
                        var isBlacklisted = await blacklistService.IsBlacklistedAsync(jti);
                        if (isBlacklisted)
                        {
                            context.Fail("Token revoked");
                            return;
                        }
                        
                        var securityService = context.HttpContext
                            .RequestServices
                            .GetRequiredService<ISecurityStampService>();

                        var currentStamp =
                            await securityService.GetSecurityStampAsync(
                                Guid.Parse(userId!));

                        if (currentStamp != tokenSecurityStamp)
                        {
                            context.Fail("Security stamp invalid");
                            return;
                        }
                    }
                };
                
            });

        services.AddAuthorization();
        return services;
    }
}