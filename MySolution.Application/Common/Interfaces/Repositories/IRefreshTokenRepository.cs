using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
 
    Task<RefreshToken?> GetByHashAsync(string tokenHash);
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<List<RefreshToken>> GetValidTokenAsync(Guid userId);
    Task RevokeAsync(Guid userId);
    Task <int> CleanUpExpiredTokensAsync(int revokedBefore);
    Task RevokeByJtiAsync(string jti);
}