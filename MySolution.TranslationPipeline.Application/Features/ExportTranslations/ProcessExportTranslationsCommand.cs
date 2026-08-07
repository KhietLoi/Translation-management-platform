using MediatR;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.TranslationJob.Application.Features.ExportTranslations;

public class ProcessExportTranslationsCommand : IRequest
{
    public ExportTranslationsEvent Message { get; set; } = null!;
}