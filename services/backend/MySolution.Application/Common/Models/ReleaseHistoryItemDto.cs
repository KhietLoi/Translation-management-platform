namespace MySolution.Application.Common.Models;

public class ReleaseHistoryItemDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public int Version { get; set; }
    public string? DownloadUrl { get; set; } 
    public string? BlobFileName { get; set; } 
    public int TotalKey { get; set; }
    public DateTime PublishedAt { get; set; }
    public Guid PublishedBy { get; set; } 
    public string PublishedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}