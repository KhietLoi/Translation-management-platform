namespace MySolution.Application.Common.Models;

public class PasswordResetPayload
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int PasswordVersion { get; set; } 
    public DateTime ExpiredAt { get; set; }
}