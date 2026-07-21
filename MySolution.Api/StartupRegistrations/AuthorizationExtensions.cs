using Microsoft.AspNetCore.Authorization;
using MySolution.Api.Authorization;

namespace MySolution.Api.StartupRegistrations;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationLayer(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        
        return services;
    }
    
}