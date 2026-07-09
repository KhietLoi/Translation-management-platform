namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Request model for assigning a role to a user
/// </summary>
public class AssignRoleToUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}