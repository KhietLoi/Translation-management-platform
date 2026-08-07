using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Infrastructure.MassTransit.Consumers;
using MySolution.Infrastructure.Options;
using Shared.MassTransit;
using Shared.MassTransit.IntegrationEvents;


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
            x.AddConsumer<ExportTranslationsConsumer>();
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
                ConfigureEmailQueues(context, cfg);
              });
        });
        services.AddScoped<IMessageSender, SendEndPointCustomProvider>();

        return services;
    }

    private static void ConfigureEmailQueues(
        IBusRegistrationContext context,
        IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<ExportTranslationsEvent>(),
            e =>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<
                    ExportTranslationsConsumer>(context);
            });
    }
    //Retry RabbitMq:
    private static void ConfigureRetry(IRabbitMqReceiveEndpointConfigurator endpoint)
    {
        endpoint.UseMessageRetry(r =>
        {
            r.Interval(
                3,
                TimeSpan.FromSeconds(5));
        });

        endpoint.ConcurrentMessageLimit = 5;
    }
}