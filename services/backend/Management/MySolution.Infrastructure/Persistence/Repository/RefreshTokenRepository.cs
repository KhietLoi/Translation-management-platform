using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
///     Repository for managing refresh tokens in the database.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class RefreshTokenRepository(AppDbContext context, ILogger logger)
    : Repository<RefreshToken>(context, logger), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
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
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiredAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public virtual async Task RevokeAsync(Guid userId)
    {
        await DbSet
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.RevokedAt,
                    DateTime.UtcNow));
    }

    public async Task<int> CleanUpExpiredTokensAsync(int revokedBefore)
    {
        var now = DateTime.UtcNow;
        var revokeCutoff = now.AddDays(-revokedBefore);
        return await DbSet
            .Where(x =>
                x.ExpiredAt < now ||
                (
                    x.RevokedAt != null &&
                    x.RevokedAt < revokeCutoff
                )).ExecuteDeleteAsync();
    }

    // public async Task RevokeByJtiAsync(string jti)
    // {
    //     var refreshToken = await DbSet.FirstOrDefaultAsync(x => x.Jti == jti && x.RevokedAt == null);
    //     if (refreshToken != null)
    //     {
    //         refreshToken.RevokedAt = DateTime.UtcNow;
    //         await Context.SaveChangesAsync();
    //     }
    // }
}