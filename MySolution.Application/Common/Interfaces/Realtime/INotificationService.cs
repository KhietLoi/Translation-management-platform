using MySolution.Domain.Enums;

namespace MySolution.Application.Common.Interfaces.Realtime;

public interface INotificationService
{
   
        Task NotifyUserAsync(
            Guid userId,
            Guid projectId,
            Guid? triggeredByUserId,
            string title,
            string message,
            NotificationType type,
            string? navigationUrl,
            CancellationToken cancellationToken);

        Task NotifyProjectAsync(
            Guid projectId,
            Guid? triggeredByUserId,
            string title,
            string message,
            NotificationType type,
            string? navigationUrl,
            CancellationToken cancellationToken);
    
}