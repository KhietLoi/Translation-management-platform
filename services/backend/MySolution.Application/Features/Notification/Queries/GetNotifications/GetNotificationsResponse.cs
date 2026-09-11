using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Notification.Queries.GetNotifications;

public class GetNotificationsResponse : BaseResponse <GetNotificationsData>
{
  
}

public class GetNotificationsData
{
    public List<NotificationItemResponse> Notifications { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class NotificationItemResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid TriggeredByUserId { get; set; }
    public string TriggeredByUserName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? NavigationUrl { get; set; } 
    public DateTime CreatedAt { get; set; }
}