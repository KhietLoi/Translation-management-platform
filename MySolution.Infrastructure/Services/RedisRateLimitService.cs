using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces.RateLimit;
using MySolution.Application.Common.Models.RateLimit;
using MySolution.Infrastructure.Options;
using StackExchange.Redis;

namespace MySolution.Infrastructure.Services;

public class RedisRateLimitService : IRateLimitService
{
    private readonly IDatabase _database;
    private readonly RateLimitOptions _redisOptions;

    public RedisRateLimitService(IConnectionMultiplexer redis, IOptions<RateLimitOptions> options)
    {
        _database = redis.GetDatabase();
        _redisOptions = options.Value;
    }

    public async Task<RateLimitResult> CheckAsync(RateLimitPolicy policy, CancellationToken cancellationToken = default)
    {
        // Time window:
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - (long)policy.Window.TotalSeconds;
        await _database.SortedSetRemoveRangeByScoreAsync(policy.Key, double.NegativeInfinity, windowStart);
        var count = await _database.SortedSetLengthAsync(policy.Key);
        //Reject
        if (count >= policy.PermitLimit)
            return new RateLimitResult(
                false,
                0,
                DateTimeOffset.UtcNow.Add(policy.Window));

        await _database.SortedSetAddAsync(policy.Key, Guid.CreateVersion7().ToString(), now);
        await _database.KeyExpireAsync(policy.Key, policy.Window);
        return new RateLimitResult(
            true,
            policy.PermitLimit - (int)count - 1,
            null);
    }

    public int GetPermitLimit()
    {
        return _redisOptions.PermitLimit;
    }

    public TimeSpan GetPermitWindow()
    {
        return TimeSpan.FromMinutes(_redisOptions.PermitWindow);
    }
}