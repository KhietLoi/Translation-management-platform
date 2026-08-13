using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.Realtime;

public interface INotificationService
{
    Task NotifyUserAsync(Guid userId, string title, string message,  NotificationType type, string? navigationUrl, CancellationToken cancellationToken);
    Task NotifyProjectAsync(Guid projectId, string title, string message, string createdBy,  NotificationType type, string? navigationUrl, CancellationToken cancellationToken);
}