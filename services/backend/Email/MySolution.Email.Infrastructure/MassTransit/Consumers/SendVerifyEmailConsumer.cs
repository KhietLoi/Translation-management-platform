using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendVerifyEmail;
using Shared.MassTransit.Contracts;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendVerifyEmailConsumer : IConsumer<SendVerifyEmail>
{
    private readonly IMediator _mediator;

    public SendVerifyEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<SendVerifyEmail> context)
    {
        await _mediator.Send(new SendVerifyEmailCommand { Message = context.Message.Content }, context.CancellationToken);
    }
}