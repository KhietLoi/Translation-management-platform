using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Features.TranslationPipeline.Commands.ProcessExportTranslations;
using Shared.MassTransit.Contracts;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ExportTranslationsConsumer : IConsumer<ExportTranslations>
{
    private const string EventName = "ExportTranslationsConsumer";
    private readonly ILogger<ExportTranslationsConsumer> _logger;
    private readonly IMediator _mediator;

    public ExportTranslationsConsumer(ILogger<ExportTranslationsConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ExportTranslations> context)
    {
        var message = context.Message;
        _logger.LogInformation("Export message received. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);

        try
        {
            await _mediator.Send(new ProcessExportTranslationsCommand{ Message = message.Content}, context.CancellationToken);
            _logger.LogInformation("Export message processed successfully. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing export message. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
    }
}