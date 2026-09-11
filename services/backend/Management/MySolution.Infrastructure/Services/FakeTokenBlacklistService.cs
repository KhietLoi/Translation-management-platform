using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Infrastructure.Services;

public class FakeTokenBlacklistService : ITokenBlacklistService
{
    public Task BlacklistAsync(string jti, TimeSpan ttl)
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsBlacklistedAsync(string jti)
    {
        return Task.FromResult(false);
    }
}