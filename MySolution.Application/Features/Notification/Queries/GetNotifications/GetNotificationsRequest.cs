namespace MySolution.Application.Features.Notification.Queries.GetNotifications;

public class GetNotificationsRequest
{
    public Guid? ProjectId { get; set; }
    public bool? IsRead { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}