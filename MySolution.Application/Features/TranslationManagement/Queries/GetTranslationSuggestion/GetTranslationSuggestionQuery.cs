using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationSuggestion;

public class GetTranslationSuggestionQuery : IRequest<GetTranslationSuggestionResponse>
{
    public GetTranslationSuggestionRequest Payload { get; set; }

    public GetTranslationSuggestionQuery(GetTranslationSuggestionRequest payload)
    {
        Payload = payload;
    }
}