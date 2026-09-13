using MediatR;

namespace MySolution.Application.Features.Admin.User.Queries.GetUser;

public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public GetUsersQuery(GetUsersRequest payload)
    {
        Payload = payload;
    }

    public GetUsersRequest Payload { get; }
}