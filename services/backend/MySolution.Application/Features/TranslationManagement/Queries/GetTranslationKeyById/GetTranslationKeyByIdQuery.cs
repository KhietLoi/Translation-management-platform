using MediatR;
using MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdQuery : IRequest<GetTranslationKeyByIdResponse>
{
    public Guid Id { get; set; }
    public GetTranslationKeyByIdQuery(Guid id)
    {
        Id = id;
    }
}