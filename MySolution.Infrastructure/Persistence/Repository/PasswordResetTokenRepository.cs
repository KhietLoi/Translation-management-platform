using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class PasswordResetTokenRepository (AppDbContext context, ILogger logger) : Repository<PasswordResetToken>(context, logger), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<List<PasswordResetToken>> GetActiveTokensByUserIdAsync(Guid userId)
    {
        return await context.PasswordResetTokens
            .Where(x =>
                x.UserId == userId &&
                !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }
}