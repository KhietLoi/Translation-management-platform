using System.Text.Json.Serialization;

namespace MySolution.Application.Common.Models.AI;

public class TranslationSuggestionResponse
{
    [JsonPropertyName("suggestion")]
    public string Suggestion { get; init; } = string.Empty;
}