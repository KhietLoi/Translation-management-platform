using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

public class GetTranslationJobResponse : BaseResponse <GetTranslationJobResponseData>
{

}

public class GetTranslationJobResponseData
{
    public List<GetTranslationJobHistoryItem> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetTranslationJobHistoryItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? LanguageId { get; set; }
    public Guid? NamespaceId { get; set; }
    public TranslationJobType  TranslationJobType { get; set; }
    public TranslationJobStatus Status { get; set; }
    public FileType  FileType { get; set; }
    public string? FileName { get; set; } 
    public string? DownloadUrl { get; set; } 
    public DateTime CompletedAt {get; set;}
    
    public int SuccessRecords  { get; set; }
    public int FailedRecords  { get; set; }
    public int TotalRecords  { get; set; }
    public int SkipRecords  { get; set; }
}