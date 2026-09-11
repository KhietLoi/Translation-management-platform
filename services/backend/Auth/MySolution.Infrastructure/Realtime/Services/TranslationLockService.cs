using System.Collections.Concurrent;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Models.Realtime;

namespace MySolution.Infrastructure.Realtime.Services;

public class TranslationLockService : ITranslationLockService
{
    private static readonly ConcurrentDictionary<Guid, TranslationsLockInfo> Locks = new();

    public Task<bool> AcquireLockAsync(Guid translationValueId, Guid userId, string username, string connectionId)
    {
        var acquired = Locks.TryAdd(
            translationValueId,
            new TranslationsLockInfo
            {
                TranslationValueId = translationValueId,
                UserId = userId,
                Username = username,
                ConnectionId = connectionId,
                LockedAt = DateTime.UtcNow
            });

        return Task.FromResult(acquired);
    }

    public Task ReleaseLockAsync(Guid translationValueId, Guid userId)
    {
        if (Locks.TryGetValue(translationValueId, out var current))
        {
            if (current.UserId == userId)
            {
                Locks.TryRemove(translationValueId, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task ReleaseAllLocksByConnectionAsync(string connectionId)
    {
        var lockedItems = Locks
            .Where(x => x.Value.ConnectionId == connectionId)
            .ToList();

        foreach (var item in lockedItems)
        {
            Locks.TryRemove(item.Key, out _);
        }

        return Task.CompletedTask;
    }

    public Task<TranslationsLockInfo?> GetLockAsync(Guid translationValueId)
    {
        Locks.TryGetValue(translationValueId, out var lockInfo);
        return Task.FromResult(lockInfo);
    }
}