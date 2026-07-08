using MediatR;

namespace MySolution.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest <DeleteRoleResponse>
{
    public Guid Id { get; set; }
    public  DeleteRoleCommand(Guid id)
    {
        Id = id;
    }
}