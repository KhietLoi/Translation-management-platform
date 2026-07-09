using MediatR;

namespace MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

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