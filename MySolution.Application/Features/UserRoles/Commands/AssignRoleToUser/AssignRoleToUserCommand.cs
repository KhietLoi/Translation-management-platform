using MediatR;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Command to assign a role to a user
/// </summary>
public class AssignRoleToUserCommand : IRequest <AssignRoleToUserResponse>
{
    public AssignRoleToUserRequest Payload { get; set; }

    public AssignRoleToUserCommand(AssignRoleToUserRequest payload)
    {
        Payload = payload;
    }
}