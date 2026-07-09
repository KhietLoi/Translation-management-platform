using MediatR;

namespace MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

/// <summary>
/// Command to remove a role from a user
/// </summary>
public class RemoveRoleFromUserCommand : IRequest <RemoveRoleFromUserResponse>
{
    public Guid RoleId { get; set; }
    public Guid UserId { get; set; }

    public RemoveRoleFromUserCommand(Guid roleId, Guid userId)
    {
        RoleId = roleId;
        UserId = userId;
    }
}