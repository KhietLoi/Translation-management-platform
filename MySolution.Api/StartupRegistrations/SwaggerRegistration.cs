namespace MySolution.Api.StartupRegistrations;

public static class SwaggerRegistration
{
    public static IServiceCollection AddSwaggerLayer(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    public static IApplicationBuilder UseSwaggerLayer(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}