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
    ITranslationValueRepository TranslationValue { get; }
    ITranslationKeyRepository TranslationKey { get; }
    IAuditLogRepository  AuditLog { get; }
    ITranslationJobRepository TranslationJob { get; }
    
    IApiKeyPermissionRepository ApiKeyPermission { get; }
    IApiKeyRepository ApiKey { get; }
    IApplicationRepository Application { get; }
    IApiKeyUsageLogRepository ApiKeyUsageLog { get; }
    
    ITranslationReleaseRepository TranslationRelease { get; }
    INotificationRepository  Notification { get; }

    Task SaveAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}