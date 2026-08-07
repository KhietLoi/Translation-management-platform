using MassTransit;
using MySolution.Application.Common.Interfaces.File;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ExportTranslationsConsumer : IConsumer <ExportTranslationsEvent>
{
    private readonly IExportService _exportService;
    
    public ExportTranslationsConsumer(IExportService exportService)
    {
        _exportService = exportService;
    }
    
    public async Task Consume(ConsumeContext<ExportTranslationsEvent> context)
    {
        await _exportService.ExportAsync(context.Message.ProjectId, context.Message.Format, context.CancellationToken);
    }
}