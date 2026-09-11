namespace MySolution.Application.Common.Models;

public class PublishStatistics
{
    public Guid ReleaseId { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }
    public int SkippedRecords { get; set; }
    public int Version { get; set; }
    public string? BlobFileName { get; set; }
    public string? DownloadUrl { get; set; }
}