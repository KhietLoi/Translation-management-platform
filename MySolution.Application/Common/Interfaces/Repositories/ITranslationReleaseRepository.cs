using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationReleaseRepository : IRepository <TranslationRelease>
{
    Task<int> GetLatestVersionAsync(Guid projectId, CancellationToken cancellationToken);
    Task<List<TranslationRelease>> GetActiveByProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task<bool> ExistReleaseWithChecksumAsync(Guid projectId, string checksum, CancellationToken cancellationToken);
    Task<(List<TranslationRelease> Items, int TotalCount)> GetReleaseHistoryAsync(Guid projectId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}