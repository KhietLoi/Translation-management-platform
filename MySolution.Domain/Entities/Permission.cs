namespace MySolution.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    //Ma Quyen:
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    //Navigation:
    //Danh sach Role de permission su dung:
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}