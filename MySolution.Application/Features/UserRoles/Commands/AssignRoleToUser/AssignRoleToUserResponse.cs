using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Response class for the assign role to user operation.
/// </summary>
public class AssignRoleToUserResponse : BaseResponse <AssignRoleToUserData>
{
}
public class AssignRoleToUserData
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    
}