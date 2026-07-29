using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ITranslationValueRepository : IRepository<TranslationValue>
{
    Task<TranslationValue?> GetByIdAsync(Guid id);
    Task<List<TranslationValue>> GetAsync(
        Guid? translationKeyId,
        Guid? languageId,
        TranslationStatus? status = TranslationStatus.Draft);
    Task<bool> ExistsAsync(Guid translationKeyId, Guid languageId, Guid? excludeId = null);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid?> GetExistingIdAsync(Guid id);
    Task<TranslationValue?> GetByIdTrackingAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}