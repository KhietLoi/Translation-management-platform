using MySolution.Application.Common.Models;
using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationReleaseRepository : IRepository <TranslationRelease>
{
    Task<TranslationRelease?> GetByIdAsync(Guid releaseId, CancellationToken cancellationToken);
    Task<TranslationRelease?> GetCurrentActiveAsync(Guid projectId, CancellationToken cancellationToken);
    Task<int> GetLatestVersionAsync(Guid projectId, CancellationToken cancellationToken);
    Task<List<TranslationRelease>> GetActiveByProjectAsync(Guid projectId, CancellationToken cancellationToken);
    Task<bool> ExistReleaseWithChecksumAsync(Guid projectId, string checksum, CancellationToken cancellationToken);
    Task<(List<ReleaseHistoryItemDto> Items, int TotalCount)> GetReleaseHistoryAsync(Guid projectId, int pageNumber,
        int pageSize, CancellationToken cancellationToken);
    Task <TranslationRelease?> GetPreviousReleaseAsync(Guid projectId, DateTime currentReleasePublishedAt, CancellationToken cancellationToken);
    Task <TranslationRelease?> GetActiveReleaseAsync(Guid projectId, CancellationToken cancellationToken);
}