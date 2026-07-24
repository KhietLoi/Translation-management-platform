using MySolution.Application.Common.Models.RateLimit;

namespace MySolution.Application.Common.Interfaces.RateLimit;

public interface IRateLimitPolicyProvider
{
    RateLimitPolicy GetPolicy(string policyName, string key);
}