namespace Shared.MassTransit.IntegrationEvents;

public class TranslationJobCompletedEmailEvent
{
    public Guid JobId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }
    public int SkippedRecords { get; set; }
}