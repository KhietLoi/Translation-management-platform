using MediatR;

namespace MySolution.Application.Features.User.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdResponse>
{
   public Guid Id { get; set; }
   public  GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
}