namespace MySolution.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenCleanupSettings
{
    int IntervalHours { get; }
    int KeepRevokedTokenDays { get; }
}