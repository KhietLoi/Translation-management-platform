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
            x.AddConsumer<SendVerifyEmailConsumer>();
            x.AddConsumer<SendSetupPasswordEmailConsumer>();
            x.AddConsumer<SendForgotPasswordEmailConsumer>();

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
            QueueNameHelper.Get<SendVerifyEmailEvent>(),
            e=>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendVerifyEmailConsumer>(context);
            });

        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<SendSetUpPasswordEmailEvent>(),
            e=>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendSetupPasswordEmailConsumer>(context);
            });

        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<SendForgotPasswordEmailEvent>(),
            e=>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendForgotPasswordEmailConsumer>(context);
            });
    }
    
    //Retry RabbitMq:
    private static void ConfigureRetry(IRabbitMqReceiveEndpointConfigurator endpoint)
    {
        endpoint.UseMessageRetry(r =>
        {
            r.Interval(
                retryCount: 3,
                interval: TimeSpan.FromSeconds(5));
        });

        endpoint.ConcurrentMessageLimit = 5;
    }
    }
    