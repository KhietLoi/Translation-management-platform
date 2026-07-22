namespace MySolution.Application.Common.Models.RateLimit;

public sealed record RateLimitPolicy(string Key, int PermitLimit, TimeSpan Window);