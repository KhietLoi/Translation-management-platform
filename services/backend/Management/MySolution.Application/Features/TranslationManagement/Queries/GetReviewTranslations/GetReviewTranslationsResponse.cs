using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetReviewTranslations;

public class GetReviewTranslationsResponse : BaseResponse <GetReviewTranslationsData>
{
}

public class GetReviewTranslationsData
{
    public List<TranslationItem> Items { get; set; } = [];
}

public class TranslationItem
{
    public Guid TranslationValueId { get; set; }
    public Guid TranslationKeyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}