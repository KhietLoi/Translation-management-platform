using MediatR;

namespace MySolution.Application.Features.Roles.Commands.UpdateRolePermissions;

public class UpdateRolePermissionsCommand : IRequest<UpdateRolePermissionsResponse>
{
    public UpdateRolePermissionsRequest Payload { get; set; }
    public  UpdateRolePermissionsCommand(UpdateRolePermissionsRequest payload)
    {
        Payload = payload;
    }
}