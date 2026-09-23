using MediatR;

namespace MySolution.Application.Features.Admin.Permission.Commands.DeletePermission;

public class DeletePermissionCommand : IRequest<DeletePermissionResponse>
{
    public Guid Id { get; }
    public DeletePermissionCommand(Guid id)
    {
        Id = id;
    }
}