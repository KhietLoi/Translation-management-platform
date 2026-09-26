
using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendTranslationJobFailedEmail;
using Shared.MassTransit.Contracts;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendTranslationJobFailedEmailConsumer : IConsumer<TranslationJobFailedEmail>
{
    private readonly IMediator _mediator;

    public SendTranslationJobFailedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<TranslationJobFailedEmail> context)
    {
        var message = context.Message.Content;
        await _mediator.Send(new SendTranslationJobFailedEmailCommand (message), context.CancellationToken);
    }
}