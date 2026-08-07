using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Persistence.Repository;

namespace MySolution.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        var logger = loggerFactory.CreateLogger("UnitOfWork");

        User = new UserRepository(_context, logger);
        Role = new RoleRepository(_context, logger);
        Permission = new PermissionRepository(_context, logger);
        RefreshToken = new RefreshTokenRepository(_context, logger);
        UserRole = new UserRoleRepository(_context, logger);
        RolePermission = new RolePermissionRepository(_context, logger);
        UserProfile = new UserProfileRepository(_context, logger);
        Project = new ProjectRepository (context, logger);
        Language = new LanguageRepository (context, logger);
        Namespace = new ProjectNamespaceRepository (context, logger);
        ProjectLanguage = new ProjectLanguageRepository (context, logger);
        ProjectMember = new ProjectMemberRepository (context, logger);
        TranslationKey = new TranslationKeyRepository(context, logger);
        TranslationValue = new TranslationValueRepository(context, logger);
        AuditLog = new AuditLogRepository(context, logger);
        ApiKeyPermission = new ApiKeyPermissionRepository(_context, logger);
        ApiKey = new ApiKeyRepository(_context, logger);
        Application = new ApplicationRepository(_context, logger);
        TranslationJob = new TranslationJobRepository(_context, logger);
        ApiKeyUsageLog = new ApiKeyUsageLogRepository(_context, logger);
    }

    /*public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync().ConfigureAwait(false);
        await _context.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }*/
    public async ValueTask DisposeAsync()
    {
        if (_transaction is not null) await _transaction.DisposeAsync().ConfigureAwait(false);

        await _context.DisposeAsync().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public IUserRepository User { get; }
    public IRoleRepository Role { get; }
    public IPermissionRepository Permission { get; }
    public IRefreshTokenRepository RefreshToken { get; }
    public IUserRoleRepository UserRole { get; }
    public IRolePermissionRepository RolePermission { get; }
    public IUserProfileRepository UserProfile { get; }
    public IProjectRepository Project { get; }
    public ILanguageRepository Language { get; }
    public IProjectNamespaceRepository Namespace { get; }
    public IProjectLanguageRepository ProjectLanguage { get; }
    public IProjectMemberRepository ProjectMember { get; }
    public ITranslationValueRepository TranslationValue { get; }
    public ITranslationKeyRepository TranslationKey { get; }
    public IAuditLogRepository AuditLog { get; }
    public ITranslationJobRepository TranslationJob { get; }
    public IApiKeyPermissionRepository ApiKeyPermission { get; }
    public IApiKeyRepository ApiKey { get; }
    public IApplicationRepository Application { get; }
    public IApiKeyUsageLogRepository ApiKeyUsageLog { get; }


    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IDbContextTransaction> OpenTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);

        return _transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");

        await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction");
        await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }
}