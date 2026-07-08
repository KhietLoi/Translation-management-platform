using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace MySolution.Infrastructure.Authentication;

public static class JwtConfiguration
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind JwtSettings từ appsettings.json
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

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
                    // Kiểm tra Issuer
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    // Kiểm tra Audience
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    // Kiểm tra thời gian hết hạn
                    ValidateLifetime = true,

                    // Kiểm tra chữ ký
                    ValidateIssuerSigningKey = true,

                    // Key dùng để verify JWT
                    IssuerSigningKey =
                        new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }
}