using MassTransit;
using MediatR;
using MySolution.Application.Features.Auth.SendVerifyEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendVerifyEmailConsumer : IConsumer<SendVerifyEmailEvent>
{
    private readonly IMediator _mediator;

    public SendVerifyEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendVerifyEmailEvent> context)
    {
        await _mediator.Send(new SendVerifyEmailCommand { Message = context.Message }, context.CancellationToken);
    }
}