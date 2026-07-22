namespace MySolution.Application.Common.Models.RateLimit;

public sealed record RateLimitRecord( bool Allowed, int RemainingRequests, DateTimeOffset? RetryAfter);