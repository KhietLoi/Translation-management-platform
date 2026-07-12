using MediatR;

namespace MySolution.Application.Features.User.Commands.UpdateUserRoles;

public class UpdateUserRolesCommand : IRequest<UpdateUserRolesResponse>
{
    public UpdateUserRolesRequest Payload { get; set; }
    
    public UpdateUserRolesCommand(UpdateUserRolesRequest payload)
    {
        Payload = payload;
    }
}