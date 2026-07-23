using MySolution.Application.Common.Models.RateLimit;

namespace MySolution.Application.Common.Interfaces.RateLimit;

public interface IRateLimitService
{
    Task<RateLimitResult> CheckAsync(RateLimitPolicy policy, CancellationToken cancellationToken = default);
    int GetPermitLimit();
    TimeSpan GetPermitWindow();
}