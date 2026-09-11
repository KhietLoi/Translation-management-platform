namespace MySolution.Application.Common.Models;

public class ReleaseDiffResult
{
    public List<ReleaseDiffItem> Added { get; set; } = [];
    public List<ReleaseDiffItem> Updated { get; set; } = [];
    public List<ReleaseDiffItem> Removed { get; set; } = [];
    public int AddedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int RemovedCount { get; set; }
}

public class ReleaseDiffItem
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}