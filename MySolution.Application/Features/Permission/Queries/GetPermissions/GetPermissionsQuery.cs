using MediatR;
using MySolution.Application.Features.Roles.Queries.GetRoles;

namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

/// <summary>
/// Query to retrieve permissions based on the provided request payload.
/// </summary>
/// <param name="payload"></param>
public class GetPermissionsQuery (GetPermissionsRequest payload) : IRequest <GetPermissionsResponse>
{ 
    public GetPermissionsRequest Payload = payload;
}