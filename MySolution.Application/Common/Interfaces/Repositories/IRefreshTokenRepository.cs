using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    //Nghiep vu rieng
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<List<RefreshToken>> GetValidTokenAsync(Guid userId);
    Task RevokeAsync(Guid userId);
}