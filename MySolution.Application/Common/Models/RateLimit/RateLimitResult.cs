namespace MySolution.Application.Common.Models.RateLimit;

public sealed record RateLimitResult( bool Allowed, int RemainingRequests, DateTimeOffset? RetryAfter);