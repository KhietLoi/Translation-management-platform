using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectNamespaceRepository (AppDbContext context, ILogger logger)
    : Repository<ProjectNamespace>(context, logger), IProjectNamespaceRepository
{
    public async Task<ProjectNamespace?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid projectId, string name, Guid? excludeNamespaceId = null)
    {
        return await DbSet.AnyAsync(x =>
            x.ProjectId == projectId &&
            x.Name == name &&
            (!excludeNamespaceId.HasValue || x.Id  != excludeNamespaceId.Value));
    }

    public async Task<List<ProjectNamespace>> GetByProjectIdAsync(Guid projectId)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
    public async Task<bool> IsNamespaceBelongsToProjectAsync(Guid namespaceId, Guid projectId)
    {
        return await DbSet.AnyAsync(x => x.Id == namespaceId && x.ProjectId == projectId);
    }
}