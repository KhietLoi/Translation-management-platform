using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Email.Application.Features.SendTranslationJobCompletedEmail;
using Shared.MassTransit.Contracts;

namespace MySolution.Email.Infrastructure.MassTransit.Consumers;

public class SendTranslationJobCompletedEmailConsumer : IConsumer<TranslationJobCompletedEmail>
{
    private const string EventName = "SendTranslationJobCompletedEmailConsumer";
    private readonly IMediator _mediator;
    private readonly ILogger<SendTranslationJobCompletedEmailConsumer> _logger;
    
    public SendTranslationJobCompletedEmailConsumer(IMediator mediator, ILogger<SendTranslationJobCompletedEmailConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<TranslationJobCompletedEmail> context)
    {
        var message = context.Message.Content;
        _logger.LogInformation("Translation job completed email message received. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);

        try
        {
            await _mediator.Send(new SendTranslationJobCompletedEmailCommand(message), context.CancellationToken);
           _logger.LogInformation("Sending translation job completed email. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred while processing translation job completed email message. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
    }
}   