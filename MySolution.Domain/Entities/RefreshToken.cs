namespace MySolution.Domain.Entities;
/// <summary>
/// Represents a refresh token used for authentication and authorization purposes.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Jti { get; set; } = null!;
    
    public bool IsExpired => ExpiredAt <= DateTime.UtcNow;
    public bool IsActive => RevokedAt == null && !IsExpired;
}