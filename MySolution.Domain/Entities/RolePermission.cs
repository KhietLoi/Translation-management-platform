namespace MySolution.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between roles and permissions.
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}