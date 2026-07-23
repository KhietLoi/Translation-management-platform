using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Persistence.Repository;

namespace MySolution.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;
    private IDbContextTransaction _transaction;

    public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger("UnitOfWork");

        User = new UserRepository(_context, _logger);
        Role = new RoleRepository(_context, _logger);
        Permission = new PermissionRepository(_context, _logger);
        RefreshToken = new RefreshTokenRepository(_context, _logger);
        UserRole = new UserRoleRepository(_context, _logger);
        RolePermission = new RolePermissionRepository(_context, _logger);
        UserProfile = new UserProfileRepository(_context, _logger);
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
        await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
    }
}