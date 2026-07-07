using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    
    //Nghiep vu rieng
   
    Task <User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    //Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<List<Role>> GetRolesAsync(Guid userId);
    Task<List<Permission>> GetPermissionsAsync(Guid userId);
    
    Task<User?> GetUserWithRolesAsync(string username);
    Task<User?> GetUserWithRolesAsync(Guid userId);
    
}