using MediatR;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdQuery : IRequest<GetTranslationKeyByIdResponse>
{
    public GetTranslationKeyByIdRequest Payload { get; set; }

    public GetTranslationKeyByIdQuery(GetTranslationKeyByIdRequest payload)
    {
        Payload = payload;
    }
}