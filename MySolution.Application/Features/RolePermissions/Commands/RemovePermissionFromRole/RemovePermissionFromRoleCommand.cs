using MediatR;

namespace MySolution.Application.Features.RolePermissions.Commands.RemovePermissionFromRole;

/// <summary>
/// Command to remove a permission from a role
/// </summary>
public class RemovePermissionFromRoleCommand : IRequest <RemovePermissionFromRoleResponse>
{
  public Guid RoleId { get; set; }
  public Guid PermissionId { get; set; }
  public RemovePermissionFromRoleCommand(Guid roleId, Guid permissionId)
  {
    RoleId = roleId;
    PermissionId = permissionId;
  }
}