using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ImportTranslations;

public class ImportTranslationsResponse : BaseResponse <ImportTranslationsData>
{

}

public class ImportTranslationsData
{
    public Guid JobId { get; set; }
    public TranslationJobStatus Status { get; set; }
}