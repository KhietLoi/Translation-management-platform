using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectNamespaceRepository : IRepository<ProjectNamespace>
{
    Task<bool> ExistsAsync(Guid projectId, string name, Guid? excludeNamespaceId = null);
}