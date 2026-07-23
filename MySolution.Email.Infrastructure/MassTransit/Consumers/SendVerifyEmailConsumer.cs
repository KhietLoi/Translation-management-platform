using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendVerifyEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

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