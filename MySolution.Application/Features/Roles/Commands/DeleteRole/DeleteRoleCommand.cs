using MediatR;

namespace MySolution.Application.Features.Roles.Commands.DeleteRole;

/// <summary>
/// Command to delete a role by its ID.
/// </summary>
public class DeleteRoleCommand : IRequest <DeleteRoleResponse>
{
    public Guid Id { get; set; }
    public  DeleteRoleCommand(Guid id)
    {
        Id = id;
    }
}