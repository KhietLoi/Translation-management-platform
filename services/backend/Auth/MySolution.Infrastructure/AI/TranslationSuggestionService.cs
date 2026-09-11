using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.AI;
using MySolution.Application.Common.Models.AI;

namespace MySolution.Infrastructure.AI;

public class TranslationSuggestionService :  ITranslationSuggestionService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TranslationSuggestionService> _logger;

    public TranslationSuggestionService(HttpClient httpClient,  ILogger<TranslationSuggestionService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TranslationSuggestionResponse> SuggestAsync(
        TranslationSuggestionRequest request,
        CancellationToken cancellationToken)
    {
        
        using var response = await _httpClient.PostAsJsonAsync("/api/review/suggest", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<TranslationSuggestionResponse>(cancellationToken);
        if (result is null || string.IsNullOrWhiteSpace(result.Suggestion))
        {
            throw new InvalidOperationException("AI service returned an empty suggestion.");
        }

        return result;
    }

    public async Task<BatchTranslationSuggestionResponse> BatchSuggestAsync(BatchTranslationSuggestionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync("/api/review/suggest-batch", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BatchTranslationSuggestionResponse>(cancellationToken: cancellationToken);
                return result ?? new BatchTranslationSuggestionResponse();
            }

            _logger.LogWarning("AI Batch API returned non-success status: {StatusCode}", response.StatusCode);
            return new BatchTranslationSuggestionResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call AI Batch Suggestion API.");
            return new BatchTranslationSuggestionResponse();
        }   
    }
}