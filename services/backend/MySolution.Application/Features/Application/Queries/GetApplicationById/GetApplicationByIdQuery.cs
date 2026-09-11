using MediatR;

namespace MySolution.Application.Features.Application.Queries.GetApplicationById;

public class GetApplicationByIdQuery : IRequest<GetApplicationByIdResponse>
{
    public Guid Id { get; set; }
    public GetApplicationByIdQuery(Guid id)
    {
        Id = id;
    }
}