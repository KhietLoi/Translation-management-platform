using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.Commands.UpdateUserRoles;


public class UpdateUserRolesResponse 
    : BaseResponse<UpdateUserRolesData>
{
}

public class UpdateUserRolesData
{
    public Guid UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public List<RoleData> Roles { get; init; } = [];
}

public class RoleData
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string? RoleDescription { get; init; }
}