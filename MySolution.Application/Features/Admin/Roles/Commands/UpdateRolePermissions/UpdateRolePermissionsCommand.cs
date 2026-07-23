using MediatR;

namespace MySolution.Application.Features.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommand : IRequest<UpdateRolePermissionsResponse>
{
    public UpdateRolePermissionsCommand(UpdateRolePermissionsRequest payload)
    {
        Payload = payload;
    }

    public UpdateRolePermissionsRequest Payload { get; set; }
}