using Microsoft.EntityFrameworkCore.Storage;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork
{
    IUserRepository User { get; }
    IRoleRepository Role { get; }
    IPermissionRepository Permission { get; }
    IRefreshTokenRepository RefreshToken { get; }
    IUserRoleRepository UserRole { get; }
    IRolePermissionRepository RolePermission { get; }
    
    Task SaveAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}