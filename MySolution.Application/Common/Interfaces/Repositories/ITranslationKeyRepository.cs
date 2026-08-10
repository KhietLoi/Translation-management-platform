using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationKeyRepository : IRepository<TranslationKey>
{
    Task<TranslationKey?> GetByIdAsync(Guid id);
    Task<Guid?> GetExistingIdAsync(Guid id);
    Task<TranslationKey?> GetByIdTrackingAsync(Guid id);
    Task<List<TranslationKey>> GetAsync (Guid? projectId, Guid? namespaceId, string? keyword);
    Task<bool> ExistsAsync(Guid projectId, Guid namespaceId, string key, Guid? excludeKeyId = null);
    Task<bool> ExistsAsync(Guid id);
    
    //Get project id from Translation key:
    Task<Guid> GetProjectIdAsync(Guid translationKeyId);
    // Get translation keys by project id and namespace id
    Task <(List<TranslationKey> Items, int TotalCount)> GetByGridAsync(
        Guid? projectId,
        Guid? namespaceId,
        string? keyword,
        TranslationStatus? status,
        int numberOfLanguages,
        int pageNumber,
        int pageSize
    );
    
    // Get translation keys by project id with translation values
    Task<List<TranslationKey>> GetByProjectWithTranslationValuesAsync(Guid projectId, CancellationToken cancellationToken);
    
    Task<List<TranslationKey>>
        GetByProjectAndNamespaceWithTranslationValuesAsync(
            Guid projectId,
            Guid namespaceId,
            CancellationToken cancellationToken);
    
    Task<List<TranslationKey>> GetPublishedTranslationsByProjectAsync(Guid projectId, CancellationToken cancellationToken);
}