using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public AuditAction Action { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityId { get; set; } 
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
}