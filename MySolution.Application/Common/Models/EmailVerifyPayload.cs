namespace MySolution.Application.Common.Models;

public sealed class EmailVerifyPayload
{
    public Guid UserId { get; set; }
    public string Email { get; set; } =  string.Empty;
    public DateTime ExpiredAt { get; set; } 
}