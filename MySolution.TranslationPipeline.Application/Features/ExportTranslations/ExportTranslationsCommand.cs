using MediatR;

namespace MySolution.TranslationPipeline.Application.Features.ExportTranslations;

public class ExportTranslationsCommand : IRequest<ExportTranslationsResponse>
{
    public ExportTranslationsRequest Payload { get; set; }

    public ExportTranslationsCommand(ExportTranslationsRequest payload)
    {
        Payload = payload;
    }
}