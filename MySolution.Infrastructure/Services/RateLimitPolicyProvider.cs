using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces.RateLimit;
using MySolution.Application.Common.Models.RateLimit;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.Services;

public class RateLimitPolicyProvider : IRateLimitPolicyProvider
{
    private readonly RateLimitOptions _options;

    public RateLimitPolicyProvider(IOptions<RateLimitOptions> options)
    {
        _options = options.Value;
    }

    public RateLimitPolicy GetPolicy(string policyName, string key)
    {
        if (!_options.Policies.TryGetValue(policyName, out var rule))
        {
            throw new InvalidOperationException($"Policy '{policyName}' not found.");
        }

        return new RateLimitPolicy(key, rule.PermitLimit,TimeSpan.FromMinutes(rule.PermitWindowMinutes));
    }
    
}