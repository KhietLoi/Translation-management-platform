using MassTransit;
using MediatR;
using MySolution.Application.Features.Auth.SendSetUpPasswordEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendSetupPasswordEmailConsumer : IConsumer<SendSetUpPasswordEmailEvent>
{
    private readonly IMediator _mediator;

    public SendSetupPasswordEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendSetUpPasswordEmailEvent> context)
    {
        await _mediator.Send(new SendSetUpPasswordEmailCommand { Message = context.Message },
            context.CancellationToken);
    }
}