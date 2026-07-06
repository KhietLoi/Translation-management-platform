using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    //Nghiep vu rieng:
    Task <Permission?> GetPermissionByIdAsync(Guid permissionId);
    Task<Permission?> GetPermissionByCodeAsync(string code);
    Task<bool> ExistsByCodeAsync(string code);
    
    
}