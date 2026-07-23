using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    ///     Get user by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    ///     Get user by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<User?> GetByEmailAsync(string email);

    Task<bool> ExistsByEmailOrUsernameAsync(string email, string username);

    /// Get roles for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<Role>> GetRolesAsync(Guid userId);

    /// <summary>
    ///     Get permissions for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<Permission>> GetPermissionsAsync(Guid userId);

    /// <summary>
    ///     Get user with roles and permissions by username
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    Task<User?> GetUserWithRolesAsync(string username);

    /// <summary>
    ///     Get user with roles and permissions by userId
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<User?> GetUserWithRolesAsync(Guid userId);

    /// <summary>
    ///     Check if a user exists by email or username excluding a specific userId (useful for updates)
    /// </summary>
    /// <param name="email"></param>
    /// <param name="username"></param>
    /// <param name="excludeUserId"></param>
    /// <returns></returns>
    Task<bool> ExistsByEmailOrUsernameAsync(string email, string username, Guid excludeUserId);

    Task<HashSet<string>> GetUserPermissionsAsync(Guid userId);
}