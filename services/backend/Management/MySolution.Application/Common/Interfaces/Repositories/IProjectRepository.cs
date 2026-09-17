using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<bool> ExistsByNameAsync(string name, Guid? excludeProjectId = null);
}