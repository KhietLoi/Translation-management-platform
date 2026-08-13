using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class NotificationRepository (AppDbContext context, ILogger logger)
    : Repository<Notification> (context, logger), INotificationRepository
{
    public async Task<List<Notification>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await DbSet
            .CountAsync(
                x => x.UserId == userId &&
                     !x.IsRead,
                cancellationToken);
    }

    public async Task<List<Notification>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken)
    {
        await DbSet
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsRead)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(x => x.IsRead, true),
                cancellationToken);
    }
}