using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetBatchTranslationSuggestion;

public class GetBatchTranslationSuggestionQuery : IRequest<GetBatchTranslationSuggestionResponse>
{
    public GetBatchTranslationSuggestionRequest Payload { get; set; }

    public GetBatchTranslationSuggestionQuery(GetBatchTranslationSuggestionRequest payload)
    {
        Payload = payload;
    }
}