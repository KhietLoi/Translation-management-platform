using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Infrastructure.Options;


namespace MySolution.Infrastructure.MassTransit;

public static class MassTransitRegistration
{
    public static IServiceCollection AddMassTransitServices
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var rabbitMqOptions = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>() ?? throw new InvalidOperationException("RabbitMQ configuration missing");

        services.AddMassTransit(x =>
        {

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    rabbitMqOptions.Host,
                    "/",
                    h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                    });
              });
        });
        services.AddScoped<IMessageSender, SendEndPointCustomProvider>();

        return services;
    }
}