using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class RoleRepository (AppDbContext context, ILogger logger) : Repository<Role>(context, logger), IRoleRepository
{
    // Nghiep vu rieng:
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Name == name) != null;
    }
    public async Task<List<Permission>> GetPermissionsAsync(Guid roleId)
    {
        return await _context.RolePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.Permission)
            .Distinct()
            .ToListAsync();
    }
}