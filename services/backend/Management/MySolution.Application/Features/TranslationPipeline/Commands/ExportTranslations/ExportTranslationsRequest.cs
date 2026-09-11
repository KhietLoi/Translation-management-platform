using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ExportTranslations;

public class ExportTranslationsRequest
{
    public Guid ProjectId { get; set; }
    public FileType Format { get; set; }
}