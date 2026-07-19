using MassTransit;
using MySolution.Application.Service;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendVerifyEmailConsumer : IConsumer<SendVerifyEmailEvent>
{
    private readonly IMessageBusService _messageBusService;
    
    public SendVerifyEmailConsumer(IMessageBusService messageBusService)
    {
        _messageBusService = messageBusService;
    }
    
    public async Task Consume(ConsumeContext<SendVerifyEmailEvent> context)
    {
        await _messageBusService.SendVerifyEmailAsync(context.Message, context.CancellationToken);
    }
}