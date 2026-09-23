using MediatR;

namespace MySolution.Application.Features.Admin.Permission.Commands.UpdatePermission;

public class UpdatePermissionCommand : IRequest<UpdatePermissionResponse>
{
    public Guid Id { get; set; }
    public UpdatePermissionRequest Payload { get; set; }
    
    public UpdatePermissionCommand(Guid id, UpdatePermissionRequest payload)
    {
        Id = id;
        Payload = payload;
    }
}