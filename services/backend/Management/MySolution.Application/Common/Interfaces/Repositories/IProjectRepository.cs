using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdAsync(Guid id);
    // Task <bool> ExistsAsync(Guid id);
    Task<Project?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeProjectId = null);
    Task<List<Guid>> GetAccessibleProjectIdsAsync(Guid userId, CancellationToken cancellationToken);
}