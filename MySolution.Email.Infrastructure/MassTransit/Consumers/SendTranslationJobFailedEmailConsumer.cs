
using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendTranslationJobFailedEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendTranslationJobFailedEmailConsumer : IConsumer<TranslationJobFailedEmailEvent>
{
    private readonly IMediator _mediator;

    public SendTranslationJobFailedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<TranslationJobFailedEmailEvent> context)
    {
        await _mediator.Send(new SendTranslationJobFailedEmailCommand { Message = context.Message }, context.CancellationToken);
    }
}