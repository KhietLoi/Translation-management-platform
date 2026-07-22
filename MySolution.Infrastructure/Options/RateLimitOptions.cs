namespace MySolution.Infrastructure.Options;

public class RateLimitOptions
{
    public const string PolicyKey = "RateLimitPolicy";
    public int PermitLimit { get; init; }
    public int PermitWindow { get; init; }
}