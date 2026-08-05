using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApiKeyRepository (AppDbContext context, ILogger logger) : Repository<ApiKey>(context, logger), IApiKeyRepository
{
    public async Task<ApiKey?> GetByIdAsync(Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> IsApiKeyExistsAsync(Guid applicationId, string name, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(
            x => x.ApplicationId == applicationId &&
                 x.Name == name &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }

    public async Task<ApiKey?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}