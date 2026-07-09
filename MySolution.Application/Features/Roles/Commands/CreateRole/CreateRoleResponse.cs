using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Roles.Commands.CreateRole;

/// <summary>
/// Response class for the create role operation.
/// </summary>
public class CreateRoleResponse : BaseResponse <CreateRoleData>
{
}
public class CreateRoleData
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public DateTime CreatedAt { get; set; }
}