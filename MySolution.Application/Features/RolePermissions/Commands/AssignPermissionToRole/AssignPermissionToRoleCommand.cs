using MediatR;

namespace MySolution.Application.Features.RolePermissions.Commands.AssignPermissionToRole;

/// <summary>
/// Command to assign a permission to a role
/// </summary>
public class AssignPermissionToRoleCommand : IRequest <AssignPermissionToRoleResponse>
{
    public AssignPermissionToRoleRequest Payload { get; set; }
    public AssignPermissionToRoleCommand(AssignPermissionToRoleRequest payload)
    {
        Payload = payload;
    }
}