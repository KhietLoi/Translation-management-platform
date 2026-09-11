namespace MySolution.Email.Application.Common.Models;

public class TranslationJobCompletedEmailModel
{
    public string UserName { get; init; } = default!;
    public string JobType { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public string DownloadUrl { get; init; } = default!;
    public string ProjectName { get; init; } = default!;
    public int TotalRecords { get; init; }
    public int SuccessRecords { get; init; }
    public int FailedRecords { get; init; }
    public int SkippedRecords { get; init; }
    public string LogoUrl { get; init; } = default!;
    public int Year { get; init; }
}