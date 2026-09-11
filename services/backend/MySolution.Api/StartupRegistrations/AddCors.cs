namespace MySolution.Api.StartupRegistrations;

public static class CorsRegistration
{
    public static IServiceCollection AddCorsLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["CorsAllowedOrigins"]?.Split(",");
        services.AddCors(options =>
        {
            options.AddPolicy("_allowSpecificOrigins",
                builder =>
                {
                    if (origins != null)
                        builder
                            .WithOrigins(origins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
        });

        return services;
    }
}