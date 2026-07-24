namespace MySolution.Application.Common.Interfaces.RateLimit;

public interface IRateLimitedRequest
{
    string PolicyName { get; }
    string RateLimitKey { get; }
}