using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendForgotPasswordEmail;
using Shared.MassTransit.Contracts;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendForgotPasswordEmailConsumer : IConsumer<SendForgotPasswordEmail>
{
    private readonly IMediator _mediator;

    public SendForgotPasswordEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendForgotPasswordEmail> context)
    {
        await _mediator.Send(new SendForgotPasswordEmailCommand (context.Message.Content), context.CancellationToken);
    }
}