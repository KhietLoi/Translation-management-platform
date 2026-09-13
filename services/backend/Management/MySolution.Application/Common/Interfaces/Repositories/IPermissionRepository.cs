using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>
    ///     Gets a permission by its unique identifier.
    /// </summary>
    /// <param name="permissionId"></param>
    /// <returns></returns>
        // Task<Permission?> GetPermissionByIdAsync(Guid permissionId);

    /// <summary>
    ///     Gets a permission by its code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    // Task<Permission?> GetPermissionByCodeAsync(string code);

    /// <summary>
    ///     Checks if a permission exists by its code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<bool> ExistsByCodeAsync(string code);

    /// <summary>
    ///     Gets a list of permissions by their unique identifiers.
    /// </summary>
    /// <param name="ids"></param>
    // /// <returns></returns>
    // Task<List<Permission>> GetByIdsAsync(List<Guid> ids);
}