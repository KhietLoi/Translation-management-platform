using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class EmailVerificationTokenRepository (AppDbContext context, ILogger logger) : Repository<EmailVerificationToken>(context, logger), IEmailVerificationTokenRepository
{
    public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
    {
        return await context.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task<List<EmailVerificationToken>> GetActiveTokensByUserId(Guid userId)
    {
        return await context.EmailVerificationTokens
            .Where(x => 
                x.UserId == userId &&
                !x.IsUsed && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }
}