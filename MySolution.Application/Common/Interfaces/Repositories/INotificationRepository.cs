using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<int> CountUnreadAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Notification>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken);
    Task<(List<Notification> Notifications, int TotalCount)>
        GetAsync(Guid userId,Guid? projectId, bool? isRead, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task <Notification?> GetUserNotificationAsync (Guid notificationId, Guid userId, CancellationToken cancellationToken);
}