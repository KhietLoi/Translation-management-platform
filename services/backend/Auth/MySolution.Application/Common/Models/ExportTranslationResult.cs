namespace MySolution.Application.Common.Models;

public class ExportTranslationResult
{
    public string FileName { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = null!;
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }
    public int SkippedRecords { get; set; }
}