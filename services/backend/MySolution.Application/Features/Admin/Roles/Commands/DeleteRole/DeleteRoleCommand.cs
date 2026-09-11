using MediatR;

namespace MySolution.Application.Features.Admin.Roles.Commands.DeleteRole;

/// <summary>
///     Command to delete a role by its ID.
/// </summary>
public class DeleteRoleCommand : IRequest<DeleteRoleResponse>
{
    public DeleteRoleCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}