using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class PermissionRepository (AppDbContext context, ILogger logger) : Repository<Permission> (context, logger), IPermissionRepository
{
    public virtual async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
        return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == permissionId);
    }

    public virtual async Task<Permission?> GetPermissionByCodeAsync(string code)
    {
        return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code );
    }

    public virtual async Task<bool> ExistsByCodeAsync(string code)
    {
        return await  _dbSet
                .AsNoTracking()
                .AnyAsync(x => x.Code == code);
    }
}