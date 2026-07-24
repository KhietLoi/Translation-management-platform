namespace MySolution.Infrastructure.Options;

public class RateLimitOptions
{
    public const string SectionName = "RateLimit";
    public Dictionary<string, RateLimitRule> Policies
    {
        get;
        init;
    } = new();
}
public sealed class RateLimitRule
{
    public int PermitLimit { get; init; }
    public int PermitWindowMinutes { get; init; }
}
