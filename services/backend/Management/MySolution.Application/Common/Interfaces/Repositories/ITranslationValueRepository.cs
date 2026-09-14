using MySolution.Application.Common.Models;
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
    
    // Task<List<TranslationValue>> GetForBatchTranslationAsync(
    //     List<Guid> translationValueIds,
    //     Guid projectId,
    //     Guid languageId,
    //     Guid namespaceId);
    
    Task<TranslationDashboardStats> GetDashboardStatsAsync(IReadOnlyCollection<Guid> projectIds, CancellationToken cancellationToken);
    Task<List<LanguageProgressDto>> GetLanguageProgressAsync(IReadOnlyCollection<Guid> projectIds, CancellationToken cancellationToken);
    
    //AI:
    // Task<TranslationValue?> GetForAiSuggestionAsync(Guid translationValueId, CancellationToken cancellationToken);
    // Task<List<TranslationValue>> GetListForAiSuggestionAsync(List<Guid> ids, CancellationToken cancellationToken);
    // Task<List<PendingNamespaceCount>> GetPendingNamespaceCountsAsync(Guid projectId, CancellationToken cancellationToken);

    // Task<List<PendingLanguageCount>> GetPendingLanguageCountsAsync(Guid projectId, Guid namespaceId, CancellationToken cancellationToken);
}
  