using MySolution.Application.Common.Models;
using MySolution.Application.Features.Admin.User.Queries.GetUserById;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsResponse : BaseResponse<UpdateRolePermissionsData>
{
}

public class UpdateRolePermissionsData
{
    public Guid RoleId { get; init; }

    public string RoleName { get; init; } = string.Empty;

    public List<PermissionData> Permissions { get; init; } = [];
}

public class PermissionItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}