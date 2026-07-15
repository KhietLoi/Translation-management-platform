namespace MySolution.Domain.Entities;

public class PasswordResetToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsUsed { get; set; }
    public User User { get; set; } = null!;
}