namespace MySolution.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    //Active Account
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    
    //Navigation:
    //Danh Sach Role cua User
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
   
    
  
    
}