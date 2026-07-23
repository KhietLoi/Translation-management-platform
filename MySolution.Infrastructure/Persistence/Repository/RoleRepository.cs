using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
///     Represents the repository for managing Role entities in the database context.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class RoleRepository(AppDbContext context, ILogger logger) : Repository<Role>(context, logger), IRoleRepository
{
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => EF.Functions.ILike(x.Name, name)); //Use ILike of PostgreSQL
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(x => EF.Functions.ILike(x.Name, name)) != null;
    }

    public async Task<List<Permission>> GetPermissionsAsync(Guid roleId)
    {
        return await Context.RolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.Permission)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<Role>> GetByIdsAsync(List<Guid> ids)
    {
        return await DbSet.Where(r => ids.Contains(r.Id)).ToListAsync();
    }
}