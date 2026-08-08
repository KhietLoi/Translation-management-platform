using MassTransit;
using MediatR;
using MySolution.Application.Features.ImportExport.Commands.ProcessImportTranslations;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class ImportTranslationsConsumer : IConsumer<ImportTranslationsEvent>
{
    private IMediator _mediator;
    
    public ImportTranslationsConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<ImportTranslationsEvent> context)
    {
        await _mediator.Send(new ProcessImportTranslationsCommand( context.Message.JobId), context.CancellationToken);
    }
}