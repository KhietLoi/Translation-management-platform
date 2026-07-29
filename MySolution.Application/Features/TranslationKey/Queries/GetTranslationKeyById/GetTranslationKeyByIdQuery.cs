using MediatR;

namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdQuery : IRequest<GetTranslationKeyByIdResponse>
{
    public Guid Id { get; set; }
    public GetTranslationKeyByIdQuery(Guid id)
    {
        Id = id;
    }
}