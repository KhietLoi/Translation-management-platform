namespace MySolution.Application.Common.Models.Realtime;

public class OnlineUser
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
}