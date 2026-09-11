namespace MySolution.Application.Features.TranslationManagement.Queries.GetBatchTranslationSuggestion;

public class GetBatchTranslationSuggestionRequest
{
    public List<Guid> TranslationValueIds { get; set; } = [];
}