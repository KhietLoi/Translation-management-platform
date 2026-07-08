using MediatR;


namespace MySolution.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommand : IRequest<CreateRoleResponse>
{
    public CreateRoleRequest Payload;
    public CreateRoleCommand(CreateRoleRequest payload)
    {
        Payload = payload;
    }
}