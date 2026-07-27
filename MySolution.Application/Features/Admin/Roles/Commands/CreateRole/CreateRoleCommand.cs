using MediatR;
using MySolution.Application.Features.Admin.Roles.Commands.CreateRole;

namespace MySolution.Application.Features.Roles.Commands.CreateRole;

/// <summary>
///     Command to create a new role
/// </summary>
public class CreateRoleCommand : IRequest<CreateRoleResponse>
{
    public CreateRoleRequest Payload;

    public CreateRoleCommand(CreateRoleRequest payload)
    {
        Payload = payload;
    }
}