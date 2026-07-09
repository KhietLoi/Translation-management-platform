using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Get a refresh token by its token string.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<RefreshToken?> GetByTokenAsync(string token);
    /// <summary>
    /// Gets all refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
    /// <summary>
    /// Gets all valid refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<RefreshToken>> GetValidTokenAsync(Guid userId);
    /// <summary>
    /// Revokes all refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RevokeAsync(Guid userId);
}