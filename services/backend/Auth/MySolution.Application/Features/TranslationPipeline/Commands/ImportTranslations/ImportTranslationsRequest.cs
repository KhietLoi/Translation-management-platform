using Microsoft.AspNetCore.Http;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ImportTranslations;

public class ImportTranslationsRequest
{
    public Guid ProjectId { get; set; }
    public Guid LanguageId { get; set; }
    public Guid NamespaceId { get; set; }
    public FileType Format { get; set; }
    public IFormFile File { get; set; } = null!;
}