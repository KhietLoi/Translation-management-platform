using MySolution.Application.Common.Interfaces.DistributedLock;
using RedLockNet.SERedis;

namespace MySolution.Infrastructure.Caching.DistributedLock;

public class RedisDistributedLockService : IDistributedLockService
{
    private readonly RedLockFactory _redLockFactory;

    public RedisDistributedLockService(RedLockFactory redLockFactory)
    {
        _redLockFactory = redLockFactory;
    }
    public  async Task<IDistributedLockHandler> AcquireAsync(string key, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var redLock = await _redLockFactory.CreateLockAsync(resource: key, expiryTime: expiry);
        
        return new DistributedLockHandler(redLock);
    }
}