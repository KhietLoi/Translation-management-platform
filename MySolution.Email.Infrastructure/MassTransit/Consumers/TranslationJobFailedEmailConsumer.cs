using MassTransit;
using MassTransit.Mediator;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class TranslationJobFailedEmailConsumer : IConsumer<TranslationJobFailedEvent>
{
    private readonly IMediator _mediator;

    public TranslationJobFailedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
}