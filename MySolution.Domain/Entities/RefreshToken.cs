namespace MySolution.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    //Chuoi RefreshToken
    public string Token { get; set; } = string.Empty;
    //Thoi diem het han:
    public DateTime ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    //Danh dau Token da bi thu hoi
    public bool IsRevoked { get; set; }
    
    //Navigation:
    public Guid UserId { get; set; }
    public User User { get; set; } = new User();
    
    
}