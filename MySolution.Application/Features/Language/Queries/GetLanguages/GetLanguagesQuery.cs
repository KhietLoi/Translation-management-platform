using MediatR;

namespace MySolution.Application.Features.Language.Queries.GetLanguages;

public class GetLanguagesQuery : IRequest<GetLanguagesResponse>
{
    public GetLanguagesRequest Payload { get; set; }

    public GetLanguagesQuery(GetLanguagesRequest payload)
    {
        Payload = payload;
    }
}