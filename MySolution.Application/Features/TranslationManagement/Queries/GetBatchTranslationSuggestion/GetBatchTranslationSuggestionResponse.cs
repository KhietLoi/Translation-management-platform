using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetBatchTranslationSuggestion;

public class GetBatchTranslationSuggestionResponse : BaseResponse <GetBatchTranslationSuggestionResult>
{

}

public class GetBatchTranslationSuggestionResult
{
    public List<GetBatchTranslationSuggestionData> Data { get; set; } = new();
}

public class GetBatchTranslationSuggestionData
{
    public Guid TranslationValueId { get; init; }
    public string ReferenceLanguage { get; init; } = string.Empty;
    public string TargetLanguage { get; init; } = string.Empty;
    public string Suggestion { get; init; } = string.Empty;
}