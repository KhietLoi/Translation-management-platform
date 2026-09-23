using MediatR;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRole;

/// <summary>
///     Command to update a role
/// </summary>
public class UpdateRoleCommand : IRequest<UpdateRoleResponse>
{
    public Guid Id { get; set; }
    public UpdateRoleRequest Payload { get; set; }
    public UpdateRoleCommand(Guid id, UpdateRoleRequest payload)
    {
        Id = id;
        Payload = payload;
    }
}