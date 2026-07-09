namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

public class AssignRoleToUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}