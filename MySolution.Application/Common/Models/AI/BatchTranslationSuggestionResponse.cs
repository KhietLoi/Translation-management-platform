using System.Text.Json.Serialization;

namespace MySolution.Application.Common.Models.AI;

public class BatchTranslationSuggestionResponse
{
    [JsonPropertyName("suggestions")]
    public Dictionary<string, string> Suggestions { get; set; } = new();
}