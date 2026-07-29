using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationKeyRepository : IRepository<TranslationKey>
{
    Task<TranslationKey?> GetByIdAsync(Guid id);
    Task<Guid?> GetExistingIdAsync(Guid id);
    Task<TranslationKey?> GetByIdTrackingAsync(Guid id);
    Task<List<TranslationKey>> GetAsync (Guid? projectId, Guid? namespaceId, string? keyword);
    Task<bool> ExistsAsync(Guid projectId, Guid namespaceId, string key, Guid? excludeKeyId = null);
    
    
    //Task<TranslationKey> GetByIdTracking(Guid id);
}