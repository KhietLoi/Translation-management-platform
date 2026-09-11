using MySolution.Application.Common.Models.Realtime;

namespace MySolution.Application.Common.Interfaces.Realtime;

public interface ITranslationLockService
{
    Task <bool> AcquireLockAsync(Guid translationValueId, Guid userId, string username, string connectionId);
    Task ReleaseLockAsync(Guid translationValueId, Guid userId);
    Task ReleaseAllLocksByConnectionAsync(string connectionId);
    Task<TranslationsLockInfo?> GetLockAsync(Guid translationValueId);
}