using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationSuggestion;

public class GetTranslationSuggestionResponse : BaseResponse <GetTranslationSuggestionData>
{

}

public class GetTranslationSuggestionData
{
    public Guid TranslationValueId { get; init; }
    public string ReferenceLanguage { get; init; } = string.Empty;
    public string TargetLanguage { get; init; } = string.Empty;
    public string Suggestion { get; init; } = string.Empty;
}