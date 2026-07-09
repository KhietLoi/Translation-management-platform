using MediatR;

namespace MySolution.Application.Features.Roles.Queries.GetRoles;

/// <summary>
/// Query to retrieve a list of roles based on the provided request parameters.
/// </summary>
/// <param name="payload"></param>
public class GetRolesQuery(GetRolesRequest payload) : IRequest<GetRolesResponse>
{
    public GetRolesRequest Payload = payload;
}