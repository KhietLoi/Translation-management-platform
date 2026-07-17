using MySolution.Application.Common.Interfaces.Authentication;
using StackExchange.Redis;

namespace MySolution.Infrastructure.Services;

public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IDatabase _database;

    public TokenBlacklistService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }
    public async Task BlacklistAsync(string jti, TimeSpan ttl)
    {
        await _database.StringSetAsync($"blacklist:{jti}","1", ttl);
    }

    public async Task<bool> IsBlacklistedAsync(string jti)
    {
       return await _database.KeyExistsAsync($"blacklist:{jti}");
    }
}