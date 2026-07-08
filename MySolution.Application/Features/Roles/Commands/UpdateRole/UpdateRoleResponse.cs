using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleResponse : BaseResponse <UpdateRoleData>
{
}
public class UpdateRoleData
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; } 
    public DateTime UpdatedAt { get; set; }
}