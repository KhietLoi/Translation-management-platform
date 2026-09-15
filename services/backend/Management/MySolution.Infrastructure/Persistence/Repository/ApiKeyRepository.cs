using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApiKeyRepository (AppDbContext context, ILogger logger) : Repository<ApiKey>(context, logger), IApiKeyRepository
{
    // public async Task<ApiKey?> GetByIdAsync(Guid id)
    // {
    //     return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
    // }

    public async Task<bool> IsApiKeyExistsAsync(Guid applicationId, string name, Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(
            x => x.ApplicationId == applicationId &&
                 x.Name == name &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            cancellationToken);
    }
    //
    // public async Task<ApiKey?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default)
    // {
    //     return await DbSet
    //         .Include(x => x.Permissions)
    //         .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    // }

    public async Task<(List<GetApiKeyGridData> Items, int TotalItems)> GetGridAsync(Guid? projectId, Guid? applicationId, string? keyword, bool? isRevoked, int page, int limit,
        CancellationToken cancellationToken)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.Application)
                .ThenInclude(x => x.Project)
            .Include(x => x.Permissions)
            .AsQueryable();

        if (projectId.HasValue)
        {
            query = query.Where(x =>
                x.Application.ProjectId == projectId.Value);
        }

        if (applicationId.HasValue)
        {
            query = query.Where(x =>
                x.ApplicationId == applicationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Name.Contains(keyword) ||
                x.KeyPrefix.Contains(keyword));
        }

        if (isRevoked.HasValue)
        {
            query = isRevoked.Value
                ? query.Where(x => x.RevokedAt != null)
                : query.Where(x => x.RevokedAt == null);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(x => new GetApiKeyGridData
            {
                Id = x.Id,
                ProjectId = x.Application.ProjectId,
                ProjectName = x.Application.Project.Name,
                ApplicationId = x.ApplicationId,
                ApplicationName = x.Application.Name,
                Name = x.Name,
                KeyPrefix = x.KeyPrefix,
                Permissions = x.Permissions.Select(p => p.Permission.ToString()).ToList(),
                IsRevoked = x.RevokedAt != null,
                IsExpired = x.ExpiresAt.HasValue && x.ExpiresAt.Value < DateTime.UtcNow,
                ExpiresAt = x.ExpiresAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public async Task<ApiKey?> GetByHashAsync(string hash, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x=> x.KeyHash == hash, cancellationToken);
    }
}