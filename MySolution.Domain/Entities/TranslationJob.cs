using MySolution.Domain.Enums;
namespace MySolution.Domain.Entities;

public class TranslationJob
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public TranslationJobType Type { get; set; }
    public TranslationJobStatus Status { get; set; }
    // Export
    public FileType? ExportFormat { get; set; }
    // Import
    public string? BlobFileName { get; set; }
    // Result
    public string? FileName { get; set; }
    public string? DownloadUrl { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Project Project { get; set; } = null!;
}