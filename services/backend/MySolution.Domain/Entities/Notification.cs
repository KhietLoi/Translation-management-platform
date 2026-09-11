using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TriggeredByUserId { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } =  string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public string? NavigationUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual User? TriggeredByUser { get; set; }
    public virtual Project? Project { get; set; }
    
    public Guid? ReferenceId { get; set; }
    public NotificationReferenceType? ReferenceType { get; set; }
}