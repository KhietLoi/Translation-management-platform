using MassTransit;
using MediatR;
using MySolution.Email.Application.Features.SendTranslationJobCompletedEmail;
using Shared.MassTransit.Contracts;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendTranslationJobCompletedEmailConsumer : IConsumer<TranslationJobCompletedEmail>
{
    private readonly IMediator _mediator;
    
    public SendTranslationJobCompletedEmailConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<TranslationJobCompletedEmail> context)
    {
        var message = context.Message.Content;
        await _mediator.Send(new SendTranslationJobCompletedEmailCommand(message), context.CancellationToken);
    }
}   