using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<UserRole?> GetAsync(Guid userId, Guid roleId);
    Task<bool> ExistsAsync(Guid userId, Guid roleId);
}