using System.Text.Json.Serialization;

namespace MySolution.Application.Common.Models.AI;

public class TranslationSuggestionRequest
{
    [JsonPropertyName("source_text")]
    public string SourceText { get; init; } = string.Empty;

    [JsonPropertyName("source_language")]
    public string SourceLanguage { get; init; } = string.Empty;

    [JsonPropertyName("target_language")]
    public string TargetLanguage { get; init; } = string.Empty;

    [JsonPropertyName("context")]
    public string? Context { get; init; }
}