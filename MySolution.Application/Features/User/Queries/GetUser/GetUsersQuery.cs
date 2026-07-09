using MediatR;
using MySolution.Application.Features.User.Queries.GetUser;

namespace MySolution.Application.Features.User.Queries.GetUser;

/// <summary>
/// Query to retrieve a list of users based on the provided request parameters.
/// </summary>
public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public GetUsersRequest Payload { get; }
    
    public  GetUsersQuery(GetUsersRequest payload)
    {
        Payload = payload;
    }
}