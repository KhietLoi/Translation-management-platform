using MediatR;

namespace MySolution.Application.Features.Permission.Queries.GetPermissionById;

public class GetPermissionByIdQuery : IRequest <GetPermissionByIdResponse>
{
    public Guid PermissionId { get; set; }
    public GetPermissionByIdQuery(Guid permissionId)
    {
        PermissionId = permissionId;
    }
}