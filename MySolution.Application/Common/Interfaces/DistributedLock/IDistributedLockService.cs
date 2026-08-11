namespace MySolution.Application.Common.Interfaces.DistributedLock;

public interface IDistributedLockService
{
    Task <IDistributedLockHandler> AcquireAsync (string key, TimeSpan expiry, CancellationToken cancellationToken = default);
}