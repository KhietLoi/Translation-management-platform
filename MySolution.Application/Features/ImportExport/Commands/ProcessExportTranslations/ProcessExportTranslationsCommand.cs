using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.ImportExport.Commands.ProcessExportTranslations;

public class ProcessExportTranslationsCommand : IRequest
{
    public ExportTranslationsEvent Message { get; set; } = null!;
}