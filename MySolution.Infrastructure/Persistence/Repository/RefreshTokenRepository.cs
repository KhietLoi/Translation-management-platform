using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class RefreshTokenRepository(AppDbContext context, ILogger logger) : Repository<RefreshToken>(context, logger), IRefreshTokenRepository
{
    //Nghiep vu rieng:
    public virtual async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Token == token);
    }

    public virtual async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public virtual async Task<List<RefreshToken>> GetValidTokenAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiredAt > DateTime.UtcNow)
            .ToListAsync();
            
    }

    public virtual async Task RevokeAsync(Guid userId)
    {
        var tokens = await _dbSet
            .Where(x => x.UserId == userId &&
                        !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
    }
}