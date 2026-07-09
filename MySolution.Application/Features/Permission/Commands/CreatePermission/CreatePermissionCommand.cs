using MediatR;

namespace MySolution.Application.Features.Permission.Commands.CreatePermission;

public class CreatePermissionCommand : IRequest <CreatePermissionResponse>
{
    public CreatePermissionRequest Payload { get; set; }
    
    public CreatePermissionCommand(CreatePermissionRequest payload)
    {
        Payload = payload;
    }
}