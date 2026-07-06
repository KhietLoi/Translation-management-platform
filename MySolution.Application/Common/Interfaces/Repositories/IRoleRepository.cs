using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByIdAsync(Guid id);
    Task<Role?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
    Task<List<Permission>> GetPermissionsAsync(Guid roleId);
}