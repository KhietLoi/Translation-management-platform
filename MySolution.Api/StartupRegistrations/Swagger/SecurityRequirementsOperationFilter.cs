using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using MySolution.Api.Authorization.Application;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MySolution.Api.StartupRegistrations.Swagger;

public class SecurityRequirementsOperationFilter
    : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var attributes = context.MethodInfo
            .GetCustomAttributes(true)
            .Concat(context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                    ?? Enumerable.Empty<object>())
            .ToList();

        // JWT
        if (attributes.OfType<AuthorizeAttribute>().Any())
        {
            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        }

        // API KEY
        if (attributes.OfType<ApiKeyAuthorizeAttribute>().Any())
        {
            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        }
    }
}