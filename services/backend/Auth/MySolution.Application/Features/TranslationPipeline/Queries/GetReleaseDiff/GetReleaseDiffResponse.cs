using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;

public class GetReleaseDiffResponse : BaseResponse <GetReleaseDiffData>
{

}

public class GetReleaseDiffData
{
    public int AddedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int RemovedCount { get; set; }
    public List<GetReleaseDiffItem> Added { get; set; } = [];
    public List<GetReleaseDiffItem> Updated { get; set; } = [];
    public List<GetReleaseDiffItem> Removed { get; set; } = [];
}

public class GetReleaseDiffItem
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}