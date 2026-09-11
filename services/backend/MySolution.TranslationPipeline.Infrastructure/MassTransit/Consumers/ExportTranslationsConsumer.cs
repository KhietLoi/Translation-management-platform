using MassTransit;
using MediatR;
using MySolution.Application.Features.ImportExport.Commands.ProcessExportTranslations;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.TranslationJob.Infrastructure.MassTransit.Consumers;

public class ExportTranslationsConsumer : IConsumer<ExportTranslationsEvent>
{
    private readonly IMediator _mediator;

    public ExportTranslationsConsumer(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<ExportTranslationsEvent> context)
    {
        await _mediator.Send(new ProcessExportTranslationsCommand{ Message = context.Message }, context.CancellationToken);
    }
    
}

