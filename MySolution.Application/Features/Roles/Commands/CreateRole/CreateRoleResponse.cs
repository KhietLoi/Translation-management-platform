using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleResponse : BaseResponse <CreateRoleData>
{
}
public class CreateRoleData
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleDescription { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}