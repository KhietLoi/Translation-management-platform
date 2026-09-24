using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MySolution.Api.StartupRegistrations.Swagger;

public static class SwaggerRegistration
{
    public static IServiceCollection AddSwaggerLayer(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "MySolution API",
                Version = "v1"
            });
            
            // XML Documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
            
            //Operation ID
            options.CustomOperationIds(apiDescription =>
            {
                if (apiDescription.TryGetMethodInfo(out var methodInfo))
                {
                    return methodInfo.Name;
                }

                return null;
            });

            // JWT Bearer
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Nhập JWT theo format: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            // API Key
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Name = "X-API-KEY",
                Description = "Nhập API Key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

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