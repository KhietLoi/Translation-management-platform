using MediatR;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeys;

public class GetTranslationKeysQuery : IRequest<GetTranslationKeysResponse>
{
    public GetTranslationKeysRequest Payload { get; set; }

    public GetTranslationKeysQuery(GetTranslationKeysRequest payload)
    {
        Payload = payload;
    }
}