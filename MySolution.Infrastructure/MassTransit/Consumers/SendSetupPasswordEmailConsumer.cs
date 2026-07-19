using MassTransit;
using MySolution.Application.Service;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendSetupPasswordEmailConsumer : IConsumer<SendSetUpPasswordEmailEvent>
{
    private readonly IMessageBusService _messageBusService;
    
    public SendSetupPasswordEmailConsumer(IMessageBusService messageBusService)
    {
        _messageBusService = messageBusService;
    }
    public async Task Consume(ConsumeContext<SendSetUpPasswordEmailEvent> context)
    {
        await _messageBusService.SendSetupPasswordEmailAsync(context.Message, context.CancellationToken);
    }
}