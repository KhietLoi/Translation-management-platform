using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;

public class PublishTranslationsResponse : BaseResponse <PublishTranslationsData>
{
}

public class PublishTranslationsData
{
    public Guid JobId { get; set; }
}