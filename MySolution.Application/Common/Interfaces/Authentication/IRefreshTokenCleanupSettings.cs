namespace MySolution.Application.Common.Interfaces;

public interface IRefreshTokenCleanupSettings
{
    int IntervalHours { get; }
    int KeepRevokedTokenDays { get; }
}