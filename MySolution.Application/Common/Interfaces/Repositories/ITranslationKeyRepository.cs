using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationKeyRepository : IRepository<TranslationKey>
{
    Task<TranslationKey?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid projectId, Guid namespaceId, string key, Guid? excludeKeyId = null);
    Task<TranslationKey> GetByIdTracking(Guid id);
}