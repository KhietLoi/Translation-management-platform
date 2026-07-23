using MediatR;

namespace MySolution.Application.Features.Permission.Queries.GetPermissionById;

/// <summary>
///     Query to get a permission by its ID
/// </summary>
public class GetPermissionByIdQuery : IRequest<GetPermissionByIdResponse>
{
    public GetPermissionByIdQuery(Guid permissionId)
    {
        PermissionId = permissionId;
    }

    public Guid PermissionId { get; set; }
}