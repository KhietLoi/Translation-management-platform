using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces.File;

public interface IPublishService
{
    Task <PublishStatistics> PublishAsync(Guid jobId, Guid projectId, Guid publishedBy,  string? notes,  CancellationToken cancellationToken);
}