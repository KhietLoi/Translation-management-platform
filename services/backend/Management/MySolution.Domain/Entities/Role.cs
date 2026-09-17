using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

/// <summary>
///     Represents a role in the system, which can be assigned to users and associated with permissions.
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    
    //Add new scope
    public RoleScope Scope { get; set; } 
}