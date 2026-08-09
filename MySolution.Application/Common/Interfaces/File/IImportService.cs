using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface IImportService
{
    Task<ImportStatistics> ImportAsync(Guid projectId,Guid languageId, Guid namespaceId, string fileName, FileType format, CancellationToken cancellationToken);
}
