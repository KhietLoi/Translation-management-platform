using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValues;

public class GetTranslationValuesResponse : BaseResponse <GetTranslationValuesResult>
{
    
}

public class GetTranslationValuesResult
{
    public List<GetTranslationValuesData> TranslationValues { get; set; } = [];
}
public class GetTranslationValuesData
{
    public Guid Id { get; set; }
    public Guid TranslationKeyId { get; set; }
    public string TranslationKey { get; set; } = string.Empty;
    public Guid NamespaceId { get; set; }
    public string NamespaceName { get; set; } = string.Empty;
    public Guid LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public TranslationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}