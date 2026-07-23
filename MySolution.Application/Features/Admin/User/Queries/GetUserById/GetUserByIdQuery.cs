using MediatR;

namespace MySolution.Application.Features.User.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
{
    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}