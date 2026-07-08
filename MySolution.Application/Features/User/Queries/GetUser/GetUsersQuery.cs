using MediatR;
using MySolution.Application.Features.User.Queries.GetUser;

namespace MySolution.Application.Features.User.Queries.GetUser;

public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public GetUsersRequest Payload { get; }
    
    public  GetUsersQuery(GetUsersRequest payload)
    {
        Payload = payload;
    }
}