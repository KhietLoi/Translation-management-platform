using MediatR;

namespace MySolution.Application.Features.Admin.Permission.Queries.GetPermissions;

/// <summary>
///     Query to retrieve permissions based on the provided request payload.
/// </summary>
/// <param name="payload"></param>
public class GetPermissionsQuery : IRequest<GetPermissionsResponse>
{
    public GetPermissionsRequest Payload;
    
    public GetPermissionsQuery(GetPermissionsRequest payload)
    {
        Payload = payload;
    }
}