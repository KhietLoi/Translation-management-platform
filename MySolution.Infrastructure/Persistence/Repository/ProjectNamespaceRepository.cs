using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectNamespaceRepository (AppDbContext context, ILogger logger)
    : Repository<ProjectNamespace>(context, logger), IProjectNamespaceRepository
{
    public Task<ProjectNamespace?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid projectId, string name)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProjectNamespace>> GetByProjectIdAsync(Guid projectId)
    {
        throw new NotImplementedException();
    }
}