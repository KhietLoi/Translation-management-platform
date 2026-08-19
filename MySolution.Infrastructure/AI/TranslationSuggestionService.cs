using System.Net.Http.Json;
using MySolution.Application.Common.Interfaces.AI;
using MySolution.Application.Common.Models.AI;

namespace MySolution.Infrastructure.AI;

public class TranslationSuggestionService :  ITranslationSuggestionService
{
    private readonly HttpClient _httpClient;

    public TranslationSuggestionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TranslationSuggestionResponse> SuggestAsync(
        TranslationSuggestionRequest request,
        CancellationToken cancellationToken)
    {
        
        using var response = await _httpClient.PostAsJsonAsync(
            "/api/review/suggest",
            request,
            cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TranslationSuggestionResponse>(cancellationToken);

        if (result is null || string.IsNullOrWhiteSpace(result.Suggestion))
        {
            throw new InvalidOperationException("AI service returned an empty suggestion.");
        }

        return result;
    }
}