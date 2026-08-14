using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationById;

public class GetNotificationByIdResponse : BaseResponse <GetNotificationByIdData>
{

}

public class GetNotificationByIdData
{
    public Guid NotificationId { get; set; }
    public Guid TriggerByUserId { get; set; }
    public string TriggerByUserName { get; set; } = string.Empty;
    
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    
}