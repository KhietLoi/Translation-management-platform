using MediatR;

namespace MySolution.Application.Features.Admin.Roles.Queries.GetRoleById;

/// <summary>
///     Query to retrieve a role by its ID
/// </summary>
public class GetRoleByIdQuery : IRequest<GetRoleByIdResponse>
{
    public Guid RoleId { get; set; }
    public GetRoleByIdQuery(Guid roleId)
    {
        RoleId = roleId;
    }
}