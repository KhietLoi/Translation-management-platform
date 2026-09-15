using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValueById;

public class GetTranslationValueByIdQuery : IRequest<GetTranslationValueByIdResponse>
{
    public Guid Id { get; set; }
    public GetTranslationValueByIdQuery(Guid id)
    {
        Id = id;
    }
}