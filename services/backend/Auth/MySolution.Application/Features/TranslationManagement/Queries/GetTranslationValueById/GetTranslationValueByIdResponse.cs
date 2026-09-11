using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValueById;

public class GetTranslationValueByIdResponse : BaseResponse <GetTranslationValueByIdData>
{

}

public class GetTranslationValueByIdData
{
    public Guid Id { get; set; }
    public Guid TranslationKeyId { get; set; }
    public string TranslationKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid NamespaceId { get; set; }
    public string NamespaceName { get; set; } = string.Empty;
    public Guid LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string? Value { get; set; }
    public TranslationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public Guid? TranslatedBy { get; set; }
    public DateTime? TranslatedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? PublishedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
}