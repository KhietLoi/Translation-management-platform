namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsRequest
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = [];
}