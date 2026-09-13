using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    ///     Get role by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
  //  Task<Role?> GetByIdAsync(Guid id);

    /// <summary>
    ///     Get role by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>
    ///     Check if role exists by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    ///     Get permissions for a role
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    // Task<List<Permission>> GetPermissionsAsync(Guid roleId);

    /// <summary>
    ///     Get roles by a list of ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<List<Role>> GetByIdsAsync(List<Guid> ids);
}