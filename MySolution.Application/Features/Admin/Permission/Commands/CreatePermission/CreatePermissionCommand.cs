using MediatR;

namespace MySolution.Application.Features.Permission.Commands.CreatePermission;

/// <summary>
///     Command to create a new permission
/// </summary>
public class CreatePermissionCommand : IRequest<CreatePermissionResponse>
{
    public CreatePermissionCommand(CreatePermissionRequest payload)
    {
        Payload = payload;
    }

    public CreatePermissionRequest Payload { get; set; }
}