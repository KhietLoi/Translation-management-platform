namespace MySolution.Application.Features.TranslationManagement.Queries.GetReviewTranslations;

public class GetReviewTranslationsRequest
{
    public Guid ProjectId { get; set; }
    public Guid LanguageId { get; set; }
    public Guid NamespaceId { get; set; }
}