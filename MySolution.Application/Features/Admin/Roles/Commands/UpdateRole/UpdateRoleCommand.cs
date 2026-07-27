using MediatR;
using MySolution.Application.Features.Roles.Commands.UpdateRole;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRole;

/// <summary>
///     Command to update a role
/// </summary>
public class UpdateRoleCommand : IRequest<UpdateRoleResponse>
{
    public UpdateRoleCommand(Guid id, UpdateRoleRequest payload)
    {
        Id = id;
        Payload = payload;
    }

    public Guid Id { get; set; }
    public UpdateRoleRequest Payload { get; set; }
}