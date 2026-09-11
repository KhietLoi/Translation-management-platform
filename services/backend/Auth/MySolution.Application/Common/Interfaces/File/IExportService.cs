using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;


namespace MySolution.Application.Common.Interfaces.File;

public interface IExportService
{
    Task<ExportTranslationResult> ExportAsync(Guid projectId, FileType format, CancellationToken cancellationToken);
}