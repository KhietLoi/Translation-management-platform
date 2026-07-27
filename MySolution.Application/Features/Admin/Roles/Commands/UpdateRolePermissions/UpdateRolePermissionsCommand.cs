using MediatR;
using MySolution.Application.Features.Roles.Commands.UpdateRolePermissions;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommand : IRequest<UpdateRolePermissionsResponse>
{
    public UpdateRolePermissionsCommand(UpdateRolePermissionsRequest payload)
    {
        Payload = payload;
    }

    public UpdateRolePermissionsRequest Payload { get; set; }
}