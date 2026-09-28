using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySolution.Email.Infrastructure.MassTransit.Consumers;
using MySolution.Email.Infrastructure.Options;
using Shared.MassTransit;
using Shared.MassTransit.Contracts;
using Shared.MassTransit.Core;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Infrastructure.MassTransit.Registration;

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
            x.AddConsumer<SendTranslationJobCompletedEmailConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                if (!string.IsNullOrWhiteSpace(rabbitMqOptions.ConnectionString))
                {
                    if (!Uri.TryCreate(
                            rabbitMqOptions.ConnectionString,
                            UriKind.Absolute,
                            out var brokerUri)
                        || (brokerUri.Scheme != "amqp"
                            && brokerUri.Scheme != "amqps"))
                    {
                        throw new InvalidOperationException(
                            "RabbitMq:ConnectionString must be an amqp or amqps URI.");
                    }

                    cfg.Host(brokerUri);
                }
                else
                {
                    cfg.Host(
                        rabbitMqOptions.Host,
                        "/",
                        h =>
                        {
                            h.Username(rabbitMqOptions.Username);
                            h.Password(rabbitMqOptions.Password);
                        });
                }

                ConfigureEmailQueues(context, cfg);
            });
        });
        services.AddScoped<ISendEndpointCustomProvider, SendEndpointCustomProvider>();

        return services;
    }

    private static void ConfigureEmailQueues(
        IBusRegistrationContext context,
        IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<SendVerifyEmail>(),
            e =>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendVerifyEmailConsumer>(context);
            });

        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<SendSetUpPasswordEmail>(),
            e =>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendSetupPasswordEmailConsumer>(context);
            });

        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<SendForgotPasswordEmail>(),
            e =>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendForgotPasswordEmailConsumer>(context);
            });
        
        cfg.ReceiveEndpoint(
            QueueNameHelper.Get<TranslationJobCompletedEmail>(),
            e =>
            {
                ConfigureRetry(e);
                e.ConfigureConsumer<SendTranslationJobCompletedEmailConsumer>(context);
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