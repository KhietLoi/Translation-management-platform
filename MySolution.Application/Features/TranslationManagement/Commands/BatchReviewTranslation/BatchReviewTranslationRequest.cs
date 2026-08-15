using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchReviewTranslation;

public class BatchReviewTranslationRequest
{
    public Guid ProjectId { get; set; }
    public Guid LanguageId { get; set; }
    public Guid NamespaceId { get; set; }
    public List<BatchReviewTranslationItem> Items { get; set; } = [];
}

public class BatchReviewTranslationItem
{
    public Guid TranslationValueId { get; set; }
    public TranslationStatus Status { get; set; }
    public string? RejectReason { get; set; }
}