using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface IImportService
{
    Task ImportAsync(Guid projectId,Guid languageId, Guid namespaceId, string fileName, FileType format, CancellationToken cancellationToken);
}
