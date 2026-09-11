using MediatR;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ExportTranslations;

public class ExportTranslationsCommand : IRequest<ExportTranslationsResponse>
{
    public ExportTranslationsRequest Payload { get; set; }

    public ExportTranslationsCommand(ExportTranslationsRequest payload)
    {
        Payload = payload;
    }
}