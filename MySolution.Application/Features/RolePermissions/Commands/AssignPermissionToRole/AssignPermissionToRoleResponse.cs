using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.RolePermissions.Commands.AssignPermissionToRole;

/// <summary>
/// Response class for the assign permission to role operation.
/// </summary>
public class AssignPermissionToRoleResponse : BaseResponse <AssignPermissionToRoleData>
{
}
public class AssignPermissionToRoleData
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
}