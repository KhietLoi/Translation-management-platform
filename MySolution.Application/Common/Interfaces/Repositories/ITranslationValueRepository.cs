using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationValueRepository : IRepository<TranslationValue>
{
    Task<TranslationValue?> GetByIdAsync(Guid id);
    Task<List<TranslationValue>> GetAsync(
        Guid? translationKeyId,
        Guid? namespaceId,
        Guid? languageId,
        TranslationStatus? status);
    Task<bool> ExistsAsync(Guid translationKeyId, Guid languageId, Guid? excludeId = null);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid?> GetExistingIdAsync(Guid id);
    Task<TranslationValue?> GetByIdTrackingAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    
    Task<List<TranslationValue>> GetReviewTranslationsAsync(Guid projectId, Guid languageId, Guid namespaceId, CancellationToken cancellationToken);
    Task<List<TranslationValue>> GetForBatchReviewAsync(
        List<Guid> translationValueIds,
        Guid projectId,
        Guid languageId,
        Guid namespaceId);
    
    Task<List<TranslationValue>> GetBatchTranslationValuesAsync(
        Guid projectId,
        Guid languageId,
        Guid namespaceId,
        CancellationToken cancellationToken);
    
    Task<List<TranslationValue>> GetForBatchTranslationAsync(
        List<Guid> translationValueIds,
        Guid projectId,
        Guid languageId,
        Guid namespaceId);
    }
  