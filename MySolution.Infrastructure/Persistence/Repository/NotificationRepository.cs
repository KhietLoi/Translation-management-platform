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
            .CountAsync(x => x.UserId == userId && !x.IsRead, cancellationToken);
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

    public async Task<(List<Notification> Notifications, int TotalCount)>
        GetAsync(Guid userId, Guid? projectId, bool? isRead, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.TriggeredByUser)
            .Where(x => x.UserId == userId);

        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        if (isRead.HasValue)
        {
            query = query.Where(x => x.IsRead == isRead.Value);
        }

        var totalCount =
            await query.CountAsync(cancellationToken);

        var notifications =
            await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        return (notifications, totalCount);
    }

    public async Task<Notification?> GetUserNotificationAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken)
    {
        return await DbSet 
            .FirstOrDefaultAsync(x => x.UserId == userId &&
                                      x.Id == notificationId, cancellationToken);
    }
    
}