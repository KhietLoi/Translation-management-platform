using MediatR;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

public class AssignRoleToUserCommand : IRequest <AssignRoleToUserResponse>
{
    public AssignRoleToUserRequest Payload { get; set; }

    public AssignRoleToUserCommand(AssignRoleToUserRequest payload)
    {
        Payload = payload;
    }
}