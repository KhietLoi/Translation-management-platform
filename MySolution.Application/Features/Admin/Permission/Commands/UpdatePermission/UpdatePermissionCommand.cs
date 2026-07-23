using MediatR;

namespace MySolution.Application.Features.Permission.Commands.UpdatePermission;

/// <summary>
///     Command to update a permission
/// </summary>
public class UpdatePermissionCommand : IRequest<UpdatePermissionResponse>
{
    public UpdatePermissionCommand(Guid id, UpdatePermissionRequest payload)
    {
        Id = id;
        Payload = payload;
    }

    public Guid Id { get; set; }
    public UpdatePermissionRequest Payload { get; set; }
}