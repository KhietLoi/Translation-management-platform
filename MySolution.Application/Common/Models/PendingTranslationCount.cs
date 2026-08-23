namespace MySolution.Application.Common.Models;

public class PendingNamespaceCount
{
    public Guid NamespaceId { get; set; }
    public int PendingReviewCount { get; set; }
    public int PendingUpdateCount { get; set; }
}

public class PendingLanguageCount
{
    public Guid LanguageId { get; set; }
    public int PendingReviewCount { get; set; }
    public int PendingUpdateCount { get; set; }
}