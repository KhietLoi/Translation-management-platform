using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationDetail;

public class GetNotificationDetailResponse
    : BaseResponse<GetNotificationDetailData>
{
}

public class GetNotificationDetailData
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? NavigationUrl { get; set; }
    public Guid TriggeredByUserId { get; set; }
    public string TriggeredByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public object? Detail { get; set; }
}