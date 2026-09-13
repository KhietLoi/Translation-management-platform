using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
///     Represents the repository for managing RolePermission entities in the database.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class RolePermissionRepository(AppDbContext context, ILogger logger)
    : Repository<RolePermission>(context, logger), IRolePermissionRepository
{
    public async Task<RolePermission?> GetAsync(Guid roleId, Guid permissionId)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);
    }

    public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId)
    {
        return await DbSet.AnyAsync(x => x.RoleId == roleId && x.PermissionId == permissionId);
    }

    // public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
    // {
    //     return await DbSet.Where(x => x.RoleId == roleId).ToListAsync();
    // }

    // public async Task<List<RolePermission>> GetByRoleIdWithPermissionAsync(Guid roleId)
    // {
    //     return await DbSet.Where(x => x.RoleId == roleId).Include(x => x.Permission).ToListAsync();
    // }
}