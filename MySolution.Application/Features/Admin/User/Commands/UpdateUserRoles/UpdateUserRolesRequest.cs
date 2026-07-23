namespace MySolution.Application.Features.User.Commands.UpdateUserRoles;

public class UpdateUserRolesRequest
{
    public Guid UserId { get; set; }
    public List<Guid> RoleIds { get; set; } = new();
}