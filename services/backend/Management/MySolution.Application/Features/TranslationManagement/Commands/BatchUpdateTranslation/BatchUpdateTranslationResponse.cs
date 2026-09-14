using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;

public class BatchUpdateTranslationResponse : BaseResponse <BatchUpdateTranslationData>
{
}

public class BatchUpdateTranslationData
{
    public int Total { get; set; }
    public List<BatchUpdatedTranslationValueItem> Items { get; set; } = [];
}

public class BatchUpdatedTranslationValueItem
{
    public Guid TranslationValueId { get; set; }
    public Guid TranslationKeyId { get; set; }
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = string.Empty;
    public TranslationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}