using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendTranslationJobCompletedEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendTranslationJobCompletedEmailConsumer : IConsumer<TranslationJobCompletedEmailEvent>
{
    private readonly IMediator _mediator;
    
    public SendTranslationJobCompletedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<TranslationJobCompletedEmailEvent> context)
    {
        await _mediator.Send(new SendTranslationJobCompletedEmailCommand { Message = context.Message }, context.CancellationToken);
    }
}   