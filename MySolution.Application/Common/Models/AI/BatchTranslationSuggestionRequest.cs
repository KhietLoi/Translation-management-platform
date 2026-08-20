using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MySolution.Application.Common.Models.AI;

public class BatchTranslationSuggestionRequest
{
    [JsonPropertyName("source_language")]
    public string SourceLanguage { get; init; } = string.Empty;
    
    [JsonPropertyName("target_language")]
    public string TargetLanguage { get; init; } = string.Empty;
    
    [JsonPropertyName("data")]
    public Dictionary<string, string> Data { get; set; } = new();
}