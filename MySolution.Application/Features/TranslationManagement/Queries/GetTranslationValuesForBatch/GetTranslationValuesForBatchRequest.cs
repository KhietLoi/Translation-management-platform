namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValuesForBatch;

public class GetTranslationValuesForBatchRequest
{
    public Guid ProjectId { get; set; }
    public Guid LanguageId { get; set; }
    public Guid NamespaceId { get; set; }
}