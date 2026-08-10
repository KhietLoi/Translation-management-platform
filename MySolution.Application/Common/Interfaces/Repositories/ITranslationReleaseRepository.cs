using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationReleaseRepository : IRepository <TranslationRelease>
{
    Task<int> GetLatestVersionAsync(Guid projectId, CancellationToken cancellationToken);
    Task<List<TranslationRelease>> GetActiveByProjectAsync(Guid projectId, CancellationToken cancellationToken);
}