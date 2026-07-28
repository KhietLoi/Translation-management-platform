using MediatR;

namespace MySolution.Application.Features.TranslationKey.Queries.SearchTranslationKeys;

public class SearchTranslationKeysQuery : IRequest<SearchTranslationKeysResponse>
{
    public SearchTranslationKeysRequest Payload { get; set; }

    public SearchTranslationKeysQuery(SearchTranslationKeysRequest payload)
    {
        Payload = payload;
    }
}