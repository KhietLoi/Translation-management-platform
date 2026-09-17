using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.BackgroundServices;

public class RefreshTokenCleanupHostedService : BackgroundService
{
    private readonly ILogger<RefreshTokenCleanupHostedService> _logger;
    private readonly RefreshTokenCleanupOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;

    public RefreshTokenCleanupHostedService
    (
        ILogger<RefreshTokenCleanupHostedService> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<RefreshTokenCleanupOptions> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RefreshTokenCleanupHostedService started.");

        var now = DateTime.UtcNow;
        var revokeCutoff = now.AddDays(-_options.KeepRevokedTokenDays);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var deletedCount = await unitOfWork.RefreshToken
                    .GetAll()
                    .Where(x =>
                        x.ExpiredAt < now ||
                        (x.RevokedAt != null &&
                         x.RevokedAt < revokeCutoff))
                    .ExecuteDeleteAsync(cancellationToken: stoppingToken);
                _logger.LogInformation("Refresh token cleanup completed. Deleted {Count} records.", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cleaning refresh tokens.");
            }

            await Task.Delay(TimeSpan.FromHours(_options.IntervalHours), stoppingToken);
        }
    }
}