using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IRolePermissionRepository : IRepository<RolePermission>
{
    /// <summary>
    /// Get RolePermission by roleId and permissionId
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="permissionId"></param>
    /// <returns></returns>
    Task<RolePermission?> GetAsync(Guid roleId, Guid permissionId);
    /// <summary>
    /// Check if RolePermission exists by roleId and permissionId
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="permissionId"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Guid roleId, Guid permissionId);
}