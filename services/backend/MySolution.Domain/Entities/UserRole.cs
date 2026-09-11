namespace MySolution.Domain.Entities;

/// <summary>
///     Represents the association between a user and a role in the system
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public User User { get; set; } = null!;
}