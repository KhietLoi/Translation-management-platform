namespace MySolution.Domain.Entities;

public class OrganizationMember
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    
}