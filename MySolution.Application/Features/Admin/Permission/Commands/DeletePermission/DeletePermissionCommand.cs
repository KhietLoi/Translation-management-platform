using MediatR;

namespace MySolution.Application.Features.Permission.Commands.DeletePermission;

/// <summary>
///     Command to delete a permission by its ID.
/// </summary>
public class DeletePermissionCommand : IRequest<DeletePermissionResponse>
{
    public DeletePermissionCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}