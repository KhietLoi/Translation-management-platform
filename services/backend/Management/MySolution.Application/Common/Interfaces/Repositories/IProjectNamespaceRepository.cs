using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectNamespaceRepository : IRepository<ProjectNamespace>
{
    Task<ProjectNamespace?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid projectId, string name, Guid? excludeNamespaceId = null);
    // Task<List<ProjectNamespace>> GetByProjectIdAsync(Guid projectId);
    /*Task<bool> IsNamespaceBelongsToProjectAsync(Guid namespaceId, Guid projectId);*/
    
    // Get namespace by id and project id
    // Task <ProjectNamespace?> GetByIdAndProjectIdAsync (Guid namspaceId, Guid projectId);
    
}