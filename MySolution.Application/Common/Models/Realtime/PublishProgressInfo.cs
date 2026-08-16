namespace MySolution.Application.Common.Models.Realtime;

public class PublishProgressInfo
{
    public Guid JobId { get; set; }
    public int Step { get; set; }
    public string Name { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Message { get; set; }
}