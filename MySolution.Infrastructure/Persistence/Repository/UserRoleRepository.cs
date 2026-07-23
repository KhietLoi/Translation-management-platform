using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
///     Represents the repository for managing UserRole entities in the database.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class UserRoleRepository(AppDbContext context, ILogger logger)
    : Repository<UserRole>(context, logger), IUserRoleRepository
{
    public async Task<UserRole?> GetAsync(Guid userId, Guid roleId)
    {
        return await DbSet.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid roleId)
    {
        return await DbSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<List<UserRole>> GetByUserIdAsync(Guid userId)
    {
        return await DbSet.Where(ur => ur.UserId == userId).ToListAsync();
    }

    public async Task<List<UserRole>> GetByUserIdWithRoleAsync(Guid userId)
    {
        return await DbSet.Where(ur => ur.UserId == userId).Include(ur => ur.Role).ToListAsync();
    }
}