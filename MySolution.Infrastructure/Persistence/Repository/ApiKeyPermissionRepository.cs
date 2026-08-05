using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApiKeyPermissionRepository (AppDbContext context, ILogger logger) : Repository<ApiKeyPermission>(context,logger), IApiKeyPermissionRepository
{
    public async Task<List<ApiKeyPermission>?> GetPermissionsByApiKeyId(Guid apiKeyId)
    {
        return await DbSet
            .Where(p => p.ApiKeyId == apiKeyId)
            .ToListAsync(CancellationToken.None);
    }
}