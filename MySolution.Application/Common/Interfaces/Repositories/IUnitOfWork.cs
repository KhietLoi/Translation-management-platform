using Microsoft.EntityFrameworkCore.Storage;

namespace MySolution.Application.Common.Interfaces.Repositories;

/// <summary>
///     Unit of Work interface for managing repositories and transactions
/// </summary>
public interface IUnitOfWork
{
    IUserRepository User { get; }
    IRoleRepository Role { get; }
    IPermissionRepository Permission { get; }
    IRefreshTokenRepository RefreshToken { get; }
    IUserRoleRepository UserRole { get; }
    IRolePermissionRepository RolePermission { get; }
    IUserProfileRepository UserProfile { get; }
    IProjectRepository  Project { get; }
    ILanguageRepository Language { get; }
    IProjectNamespaceRepository Namespace { get; }
    IProjectLanguageRepository ProjectLanguage { get; }
    IProjectMemberRepository ProjectMember { get; }

    Task SaveAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}