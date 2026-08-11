using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ProcessExportTranslations;

public class ProcessExportTranslationsCommand : IRequest
{
    public ExportTranslationsEvent Message { get; set; } = null!;
}