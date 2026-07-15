using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Authentication;
using MySolution.Infrastructure.BackgroundServices;
using MySolution.Infrastructure.Persistence;
using MySolution.Infrastructure.Persistence.Configurations;
using MySolution.Infrastructure.Services;

namespace MySolution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //db
        services.AddDbContext<AppDbContext>(options =>
                   {
                       options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                   });
        //jwt:
        services.AddJwtAuthentication(configuration);
        services.AddCustomServices();
        //Mail service:
        services.AddOptions<SendGridOptions>()
            .Bind(configuration.GetSection(
                SendGridOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddScoped<IEmailService, SendGridEmailService>();
        services.AddScoped<IHashService, HashService>();
        //Background Clean RefreshToken
        services.AddHostedService<RefreshTokenCleanupHostedService>();
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
        return services;
    }
}