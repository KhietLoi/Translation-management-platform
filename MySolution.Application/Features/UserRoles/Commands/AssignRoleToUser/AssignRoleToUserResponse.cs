using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

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