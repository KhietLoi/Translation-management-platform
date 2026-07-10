using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
/// Represents a repository for managing User entities in the database.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class UserRepository (AppDbContext context, ILogger logger) : Repository<User>(context, logger), IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<User?> GetByEmailAsync(string email)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Email == email);
    }
    
    public async Task <bool> ExistsByEmailOrUsernameAsync(string email, string username)
    {
        return await DbSet.AnyAsync(x => x.Email == email || x.Username == username);
    }

    public virtual async Task<List<Role>> GetRolesAsync(Guid userId)
    {
        return await Context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Role)
            .ToListAsync();
    }

    public virtual async Task<List<Permission>> GetPermissionsAsync(Guid userId)
    {
        return await Context.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission)
            .Distinct()
            .ToListAsync();
    }

    public async Task<User?> GetUserWithRolesAsync(string username)
    {
        return await DbSet
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<User?> GetUserWithRolesAsync(Guid userId)
    {
        return await Context.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x.RolePermissions)
            .ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => x.Id == userId);
    }

    /*// Check Email 
    public async Task<bool> ExistsByEmailAsync(string email, Guid excludeUserId)
    {
        return await DbSet.AnyAsync(x => x.Email == email && x.Id != excludeUserId);
    }
    //Check UserName
    public async Task<bool> ExistsByUsernameAsync(string username, Guid excludeUserId)
    {
        return await DbSet.AnyAsync(x => x.Username == username && x.Id != excludeUserId);
    }*/
    
    public async Task <bool>  ExistsByEmailOrUsernameAsync(string email, string username, Guid excludeUserId)
    {
        return await DbSet.AnyAsync(x => (x.Email == email || x.Username == username) && x.Id != excludeUserId);
    }
}