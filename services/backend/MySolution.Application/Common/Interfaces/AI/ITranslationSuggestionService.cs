using MySolution.Application.Common.Models.AI;

namespace MySolution.Application.Common.Interfaces.AI;

public interface ITranslationSuggestionService
{
    Task<TranslationSuggestionResponse> SuggestAsync(TranslationSuggestionRequest request, CancellationToken cancellationToken);
    Task<BatchTranslationSuggestionResponse> BatchSuggestAsync(BatchTranslationSuggestionRequest request, CancellationToken cancellationToken);
    
}