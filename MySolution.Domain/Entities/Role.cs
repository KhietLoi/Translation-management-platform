namespace MySolution.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    //Navigation:
    // Danh sach User thuoc Role
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    // Danh sach Permission cua Role
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    
    
}