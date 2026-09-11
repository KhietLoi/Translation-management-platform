namespace MySolution.Application.Common.Interfaces.Authentication;

public interface IPermissionCacheService
{
    Task<HashSet<string>?> GetAsync(Guid userId);
    Task SetAsync(Guid userId, HashSet<string> permissions, TimeSpan expiration);
    Task RemoveAsync(Guid userId);
}