using MassTransit;
using MySolution.Application.Features.Auth.SendForgotPasswordEmail;
using MySolution.Application.Service;
using MySolution.Application.Service.MessageBus;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendForgotPasswordEmailConsumer : IConsumer <SendForgotPasswordEmailEvent>
{
    private readonly IMessageBusService _messageBusService;

    public SendForgotPasswordEmailConsumer(IMessageBusService messageBusService)
    {
        _messageBusService = messageBusService;
    }

    public async Task Consume(ConsumeContext<SendForgotPasswordEmailEvent> context)
    {
        await _messageBusService.SendForgotPasswordEmailAsync(context.Message,context.CancellationToken);
    }
}