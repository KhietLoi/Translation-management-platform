using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MySolution.Application.Service;
using MySolution.Application.Service.MessageBus;
using MySolution.Application.Validation;

namespace MySolution.Application.ServiceRegistration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        //Add MediatR::
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });
        
        //Fluent Validation:
        services.AddValidatorsFromAssembly(assembly);
        
        //Register:
        services.AddScoped<IMessageBusService, MessageBusService>();
        return services;

    }
}