using MySolution.Application.Common.Models;
using Shared.Enums;


namespace MySolution.Application.Common.Interfaces.File;

public interface IExportService
{
    Task<ExportTranslationResult> ExportAsync(Guid projectId, ExportFileType format, CancellationToken cancellationToken);
}