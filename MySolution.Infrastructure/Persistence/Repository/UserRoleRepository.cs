using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class UserRoleRepository (AppDbContext context, ILogger logger) : Repository<UserRole> (context, logger), IUserRoleRepository
{
    public async Task<UserRole?> GetAsync(Guid userId, Guid roleId)
    {
        return await _dbSet.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid roleId)
    {
        return await _dbSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }
}