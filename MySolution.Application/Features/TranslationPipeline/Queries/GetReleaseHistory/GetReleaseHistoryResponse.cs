using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseHistory;

public class GetReleaseHistoryResponse : BaseResponse <GetReleaseHistoryData>
{
}

public class GetReleaseHistoryData
{
    public List<GetReleaseHistoryItem> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetReleaseHistoryItem
{
    public Guid ReleaseId { get; set; }
    public Guid ProjectId { get; set; }
    public int VersionNumber { get; set; }
    public Guid PublishingUserId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Notes { get; set; }
}