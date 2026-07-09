using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
/// Repository for managing refresh tokens in the database.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class RefreshTokenRepository(AppDbContext context, ILogger logger) : Repository<RefreshToken>(context, logger), IRefreshTokenRepository
{
    public virtual async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Token == token);
    }

    public virtual async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public virtual async Task<List<RefreshToken>> GetValidTokenAsync(Guid userId)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId && !x.IsRevoked && x.ExpiredAt > DateTime.UtcNow)
            .ToListAsync();
            
    }

    public virtual async Task RevokeAsync(Guid userId)
    {
        var tokens = await DbSet
            .Where(x => x.UserId == userId &&
                        !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }
    }
}