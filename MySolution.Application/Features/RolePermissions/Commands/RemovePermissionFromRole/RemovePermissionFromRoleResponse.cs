using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.RolePermissions.Commands.RemovePermissionFromRole;

/// <summary>
/// Response class for the remove permission from role operation.
/// </summary>
public class RemovePermissionFromRoleResponse : BaseResponse <RemovePermissionFromRoleData>
{
}
public class RemovePermissionFromRoleData
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}