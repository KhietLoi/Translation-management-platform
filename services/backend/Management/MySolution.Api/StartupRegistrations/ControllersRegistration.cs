namespace MySolution.Api.StartupRegistrations;

public static class ControllersRegistration
{
    public static IServiceCollection AddControllersLayer(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }
}