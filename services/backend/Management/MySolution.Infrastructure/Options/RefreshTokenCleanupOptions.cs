namespace MySolution.Infrastructure.Options;

public class RefreshTokenCleanupOptions
{
    public const string SectionName = "RefreshTokenCleanup";
    public int IntervalHours { get; set; }
    public int KeepRevokedTokenDays { get; set; }
}