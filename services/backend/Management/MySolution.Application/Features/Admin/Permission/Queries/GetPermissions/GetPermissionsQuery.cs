using MediatR;

namespace MySolution.Application.Features.Admin.Permission.Queries.GetPermissions;

public class GetPermissionsQuery : IRequest<GetPermissionsResponse>
{
    public GetPermissionsRequest Payload;
    
    public GetPermissionsQuery(GetPermissionsRequest payload)
    {
        Payload = payload;
    }
}