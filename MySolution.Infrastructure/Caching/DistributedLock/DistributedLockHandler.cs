using MySolution.Application.Common.Interfaces.DistributedLock;
using RedLockNet;

namespace MySolution.Infrastructure.Caching.DistributedLock;

public class DistributedLockHandler : IDistributedLockHandler
{
    private readonly IRedLock _redLock;

    public DistributedLockHandler(IRedLock redLock)
    {
        _redLock = redLock;
    }
    public ValueTask DisposeAsync()
    {
        _redLock.Dispose();
        return ValueTask.CompletedTask;
    }

    public bool IsRequired => _redLock.IsAcquired;
}