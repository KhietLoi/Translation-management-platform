using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class UserRepository(AppDbContext context, ILogger logger) : Repository<User>(context, logger), IUserRepository
{
    public async Task<bool> ExistsByEmailOrUsernameAsync(string email, string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(
            x => (x.Email == email || x.Username == username)
                 && (!excludeUserId.HasValue || x.Id != excludeUserId.Value), cancellationToken);
    }
}