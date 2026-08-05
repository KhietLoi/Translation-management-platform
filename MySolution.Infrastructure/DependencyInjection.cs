using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.RateLimit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Authentication;
using MySolution.Infrastructure.Authorization;
using MySolution.Infrastructure.BackgroundServices;
using MySolution.Infrastructure.MassTransit;
using MySolution.Infrastructure.Options;
using MySolution.Infrastructure.Persistence;
using MySolution.Infrastructure.Services;
using StackExchange.Redis;

namespace MySolution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //db
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        //jwt:
        services.AddJwtAuthentication(configuration);
        services.AddCustomServices();
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IHashService, HashService>();

        //Token Options: (Use for Token email)
        services.AddOptions<TokenOptions>()
            .Bind(configuration.GetSection(TokenOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<ITokenSetting, TokenSetting>();

        //Background Clean RefreshToken
        services.AddHostedService<RefreshTokenCleanupHostedService>();
        services.AddOptions<RefreshTokenCleanupOptions>()
            .Bind(configuration.GetSection(RefreshTokenCleanupOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        //Redis
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));
        var redisOptions = configuration
            .GetSection(RedisOptions.SectionName)
            .Get<RedisOptions>() ?? throw new InvalidOperationException("Redis configuration missing");
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            return ConnectionMultiplexer.Connect(
                redisOptions.ConnectionString);
        });
        services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
        //services.AddScoped<ITokenBlacklistService, FakeTokenBlacklistService>();
        //Frontend Url:
        services.AddOptions<FrontendOptions>()
            .Bind(configuration.GetSection(FrontendOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IApplicationUrlProvider, ApplicationUrlProvider>();
        //VerifyEmail:
        services.AddScoped<IEmailVerificationTokenService, EmailVerificationTokenService>();
        //Reset-password:
        services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        //MassTransit:
        services.AddMassTransitServices(configuration);
        //AzureBlob:
        services.Configure<AzureBlobOptions>(configuration.GetSection(AzureBlobOptions.SectionName));
        services.AddScoped<IAzureBlobService, AzureBlobService>();
        //Cache
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPermissionCacheService, PermissionCacheService>();
        //Rate Limited:
        services.AddScoped<IRateLimitService, RedisRateLimitService>();
        services.AddScoped<IRateLimitPolicyProvider, RateLimitPolicyProvider>();
        services.Configure<RateLimitOptions>(configuration.GetSection(RateLimitOptions.SectionName));
        //SecurityTimestamp
        services.AddScoped<ISecurityStampService, SecurityStampService>();
        //AuditLog
        services.AddScoped<IAuditLogService, AuditLogService>();
        
        
        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        //HttpContext
        services.AddHttpContextAccessor();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthCookieService,  AuthCookieService>();
        services.AddScoped<IApiKeyGenerator, ApiKeyGenerator>();

        return services;
    }
}