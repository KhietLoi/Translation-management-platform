using MySolution.Application.Common.Models.RateLimit;

namespace MySolution.Application.Common.Interfaces.RateLimit;

public interface IRateLimitedRequest
{
    RateLimitPolicy GetRateLimitPolicy();
}