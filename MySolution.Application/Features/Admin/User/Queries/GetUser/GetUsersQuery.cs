using MediatR;

namespace MySolution.Application.Features.User.Queries.GetUser;

/// <summary>
///     Query to retrieve a list of users based on the provided request parameters.
/// </summary>
public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public GetUsersQuery(GetUsersRequest payload)
    {
        Payload = payload;
    }

    public GetUsersRequest Payload { get; }
}