using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendSetUpPasswordEmail;
using Shared.MassTransit.Contracts;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendSetupPasswordEmailConsumer : IConsumer<SendSetUpPasswordEmail>
{
    private readonly IMediator _mediator;

    public SendSetupPasswordEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendSetUpPasswordEmail> context)
    {
        await _mediator.Send(new SendSetUpPasswordEmailCommand { Message = context.Message.Content }, context.CancellationToken);
    }
}