namespace MySolution.Application.Features.RolePermissions.Commands.AssignPermissionToRole;

/// <summary>
/// Request model for assigning a permission to a role
/// </summary>
public class AssignPermissionToRoleRequest
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}