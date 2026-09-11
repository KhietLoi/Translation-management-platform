using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeys;

public class GetTranslationKeysResponse : BaseResponse <GetTranslationKeysData>
{
}

public class TranslationKeyItem
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid NamespaceId { get; set; }
    public string NamespaceName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class GetTranslationKeysData
{
    public List<TranslationKeyItem> Items { get; set; } = [];
}