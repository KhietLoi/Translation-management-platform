using MassTransit;
using MediatR;
using MySolution.Application.Features.Auth.SendForgotPasswordEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendForgotPasswordEmailConsumer : IConsumer <SendForgotPasswordEmailEvent>
{
    private readonly IMediator _mediator;

    public SendForgotPasswordEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendForgotPasswordEmailEvent> context)
    {
        await _mediator.Send(new SendForgotPasswordEmailCommand { Message = context.Message }, context.CancellationToken);
    }
}