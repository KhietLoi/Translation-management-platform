namespace MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;

public class BatchUpdateTranslationRequest
{
    public Guid ProjectId { get; set; }
    public Guid LanguageId { get; set; }
    public Guid NamespaceId { get; set; }
    public bool IsSubmit { get; set; }
    public List<BatchUpdateTranslationValueItem> Items { get; set; } = [];
}

public class BatchUpdateTranslationValueItem
{
    public Guid TranslationValueId { get; set; }
    public string Value { get; set; } = string.Empty;
}
