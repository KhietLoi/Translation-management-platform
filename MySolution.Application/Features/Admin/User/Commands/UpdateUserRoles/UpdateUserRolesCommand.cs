using MediatR;

namespace MySolution.Application.Features.User.Commands.UpdateUserRoles;

public class UpdateUserRolesCommand : IRequest<UpdateUserRolesResponse>
{
    public UpdateUserRolesCommand(UpdateUserRolesRequest payload)
    {
        Payload = payload;
    }

    public UpdateUserRolesRequest Payload { get; set; }
}