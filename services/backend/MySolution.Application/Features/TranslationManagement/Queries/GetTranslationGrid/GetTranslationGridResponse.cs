using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationGrid;

public class GetTranslationGridResponse : BaseResponse <GetTranslationGridData>
{

}

public class GetTranslationGridData
{
    public int TotalCount { get; set; }
    public int NumberOfLanguages { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<TranslationGridItem> Items { get; set; } = new List<TranslationGridItem>();
}

public class TranslationGridItem
{
    public Guid TranslationKeyId { get; set; }

    public Guid NamespaceId { get; set; }

    public string NamespaceName { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<TranslationCellItem> Values { get; set; } = [];
}

public class TranslationCellItem
{
    public Guid TranslationValueId { get; set; }

    public Guid LanguageId { get; set; }

    public string LanguageCode { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public TranslationStatus Status { get; set; }

}
