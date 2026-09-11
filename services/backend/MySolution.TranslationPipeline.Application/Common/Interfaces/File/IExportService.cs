
using MySolution.TranslationPipeline.Application.Common.Models;
using Shared.Enums;

namespace MySolution.TranslationPipeline.Application.Common.Interfaces.File;

public interface IExportService
{
    Task<ExportTranslationResult> ExportAsync(Guid projectId, ExportFileType format, CancellationToken cancellationToken);
}