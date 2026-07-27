using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectNamespaceRepository : IRepository<ProjectNamespace>
{
    Task<ProjectNamespace?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid projectId, string name);
    Task<List<ProjectNamespace>> GetByProjectIdAsync(Guid projectId);
}