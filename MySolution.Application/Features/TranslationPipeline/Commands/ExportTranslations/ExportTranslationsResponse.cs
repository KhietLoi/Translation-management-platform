using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.ImportExport.Commands.ExportTranslations;

public class ExportTranslationsResponse : BaseResponse <ExportTranslationData>
{
}

public class ExportTranslationData
{
    public Guid JobId { get; set; }
}

