using MediatR;

namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleCommand : IRequest<UpdateRoleResponse>
{
    public Guid Id { get; set; }
    public UpdateRoleRequest Payload { get; set; }

    public UpdateRoleCommand(Guid id,UpdateRoleRequest payload)
    {
        Id = id;
        Payload = payload;
    }
}