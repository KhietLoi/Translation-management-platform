using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
/// Repository for managing permissions in the system.
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
public class PermissionRepository (AppDbContext context, ILogger logger) : Repository<Permission> (context, logger), IPermissionRepository
{
    public virtual async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == permissionId);
    }

    public virtual async Task<Permission?> GetPermissionByCodeAsync(string code)
    {
        return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code );
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code)
    {
        return await  DbSet
                .AsNoTracking()
                .AnyAsync(x => x.Code == code);
    }

    public virtual async Task<List<Permission>> GetByIdsAsync(List<Guid> ids)
    {
        return await DbSet
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
    }
}