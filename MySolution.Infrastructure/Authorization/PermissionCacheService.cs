using MySolution.Application.Common.Interfaces.Authentication;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MySolution.Infrastructure.Authorization;

public class PermissionCacheService : IPermissionCacheService
{
    private readonly IDatabase  _database;
    
    public PermissionCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }
    
    private static string GetKey(Guid userId) => $"permissions:{userId}";
    public async Task<HashSet<string>?> GetAsync(Guid userId)
    {
        var value = await _database.StringGetAsync(GetKey(userId));
        if (!value.HasValue)
        {
            return null;
        }
        
        return JsonConvert.DeserializeObject<HashSet<string>>(value!);
    }

    public async Task SetAsync(Guid userId, HashSet<string> permissions, TimeSpan expiration)
    {
        var value =  JsonConvert.SerializeObject(permissions);
        await _database.StringSetAsync(GetKey(userId), value, expiration);
    }

    public async Task RemoveAsync(Guid userId)
    {
        await _database.KeyDeleteAsync(GetKey(userId));
    }
}