namespace MySolution.Application.Common.Interfaces.DistributedLock;

public interface IDistributedLockHandler : IAsyncDisposable
{
    bool IsAcquired { get; }
}