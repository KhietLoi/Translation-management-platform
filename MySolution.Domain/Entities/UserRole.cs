namespace MySolution.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    
    //Navigation:
    public Role Role { get; set; } = null!;
    public User User { get; set; } = null!;
    
    
}