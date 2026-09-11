namespace MySolution.Infrastructure.Realtime.Models;

public class ProjectConnection
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
}