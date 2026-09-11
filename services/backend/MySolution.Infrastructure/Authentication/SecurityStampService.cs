using MySolution.Application.Common.Interfaces.Authentication;
using StackExchange.Redis;

namespace MySolution.Infrastructure.Authentication;

public class SecurityStampService : ISecurityStampService
{
    private readonly IDatabase _database;

    public SecurityStampService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task SetSecurityStampAsync(Guid userId, string securityStamp)
    {
        var result = await _database.StringSetAsync($"security-stamp:{userId}", securityStamp);
        Console.WriteLine(result);
        await _database.StringSetAsync($"security-stamp:{userId}", securityStamp);
    }

    public async Task<string?> GetSecurityStampAsync(Guid userId)
    {
        return await _database.StringGetAsync($"security-stamp:{userId}");
    }
}