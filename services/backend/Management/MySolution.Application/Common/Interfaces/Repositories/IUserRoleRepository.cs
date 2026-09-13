using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUserRoleRepository : IRepository<UserRole>
{
    /// <summary>
    ///     Get UserRole by userId and roleId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<UserRole?> GetAsync(Guid userId, Guid roleId);

    /// <summary>
    ///     Check if UserRole exists by userId and roleId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Guid userId, Guid roleId);

    // /// <summary>
    // ///     Get UserRoles by userId
    // /// </summary>
    // /// <param name="userId"></param>
    // /// <returns></returns>
    // Task<List<UserRole>> GetByUserIdAsync(Guid userId);

    // /// <summary>
    // ///     Get UserRoles by userId with Role included
    // /// </summary>
    // /// <param name="userId"></param>
    // /// <returns></returns>
    // Task<List<UserRole>> GetByUserIdWithRoleAsync(Guid userId);
}