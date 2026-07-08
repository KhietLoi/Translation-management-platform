using MediatR;

namespace MySolution.Application.Features.Roles.Queries.GetRoles;

public class GetRolesQuery(GetRolesRequest payload) : IRequest<GetRolesResponse>
{
    public GetRolesRequest Payload = payload;
}