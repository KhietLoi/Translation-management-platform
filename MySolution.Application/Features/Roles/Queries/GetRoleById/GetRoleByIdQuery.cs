using MediatR;

namespace MySolution.Application.Features.Roles.Queries.GetRoleById;

public class GetRoleByIdQuery : IRequest <GetRoleByIdResponse>
{
    public Guid RoleId { get; set; }

    public GetRoleByIdQuery(Guid roleId)
    {
        RoleId = roleId;
    }
}