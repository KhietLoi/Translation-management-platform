using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.File;

public interface IImportService
{
    Task ImportAsync(Guid projectId, string fileName, FileType format, CancellationToken cancellationToken);
}
