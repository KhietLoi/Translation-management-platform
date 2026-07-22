using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Response class for the update role operation.
/// </summary>
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