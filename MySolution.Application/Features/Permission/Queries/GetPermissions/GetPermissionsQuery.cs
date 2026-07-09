using MediatR;
using MySolution.Application.Features.Roles.Queries.GetRoles;

namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

public class GetPermissionsQuery (GetPermissionsRequest payload) : IRequest <GetPermissionsResponse>
{ 
    public GetPermissionsRequest Payload = payload;
}