namespace MySolution.Application.Common.Interfaces.Authentication;

public interface ITokenBlacklistService
{
    Task BlacklistAsync(string jti, TimeSpan ttl);
    Task<bool> IsBlacklistedAsync(string jti);
}