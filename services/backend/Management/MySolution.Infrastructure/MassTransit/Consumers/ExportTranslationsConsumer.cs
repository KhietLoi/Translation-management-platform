using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Features.TranslationPipeline.Commands.ProcessExportTranslations;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ExportTranslationsConsumer : IConsumer<ExportTranslationsEvent>
{
    private const string EventName = "ExportTranslationsConsumer";
    private readonly ILogger<ExportTranslationsConsumer> _logger;
    private readonly IMediator _mediator;

    public ExportTranslationsConsumer(ILogger<ExportTranslationsConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ExportTranslationsEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Export message received. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);

        try
        {
            await _mediator.Send(new ProcessExportTranslationsCommand{ Message = context.Message }, context.CancellationToken);
            _logger.LogInformation("Export message processed successfully. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing export message. EventName={EventName}, MessageId={MessageId}", EventName, context.MessageId);
        }
    }
}