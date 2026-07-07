using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class UserRepository (AppDbContext context, ILogger logger) : Repository<User>(context, logger), IUserRepository
{
    // 
    public virtual async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }

    /*public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Username == username);
    }*/

    public virtual async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email) != null;
    }

    public virtual async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _dbSet.Where(u => u.Username == username).AnyAsync();
    }

    public virtual async Task<List<Role>> GetRolesAsync(Guid userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Role)
            .ToListAsync();
    }

    public virtual async Task<List<Permission>> GetPermissionsAsync(Guid userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission)
            .Distinct()
            .ToListAsync();
    }

    public async Task<User?> GetUserWithRolesAsync(string username)
    {
        return await _dbSet
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<User?> GetUserWithRolesAsync(Guid userId)
    {
        return await _context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }
}