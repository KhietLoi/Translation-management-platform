using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Authentication;
using MySolution.Infrastructure.Persistence;

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
        return services;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        //HttpContext
        services.AddHttpContextAccessor();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    
    
    
    
}