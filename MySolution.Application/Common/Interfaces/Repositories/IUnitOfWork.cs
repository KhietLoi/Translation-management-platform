using Microsoft.EntityFrameworkCore.Storage;

namespace MySolution.Application.Common.Interfaces.Repositories;
/// <summary>
/// Unit of Work interface for managing repositories and transactions
/// </summary>
public interface IUnitOfWork
{
    IUserRepository User { get; }
    IRoleRepository Role { get; }
    IPermissionRepository Permission { get; }
    IRefreshTokenRepository RefreshToken { get; }
    IUserRoleRepository UserRole { get; }
    IRolePermissionRepository RolePermission { get; }
    IEmailVerificationTokenRepository EmailVerificationToken { get; }
    IPasswordResetTokenRepository PasswordResetToken { get; }
    
    Task SaveAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}