using MediatR;

namespace MySolution.Application.Features.Users.Queries.GetUser;

public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public GetUsersRequest Payload { get; }
    
    public  GetUsersQuery(GetUsersRequest payload)
    {
        Payload = payload;
    }
}