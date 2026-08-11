using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ExportTranslations;

public class ExportTranslationsResponse : BaseResponse <ExportTranslationData>
{
}

public class ExportTranslationData
{
    public Guid JobId { get; set; }
}

