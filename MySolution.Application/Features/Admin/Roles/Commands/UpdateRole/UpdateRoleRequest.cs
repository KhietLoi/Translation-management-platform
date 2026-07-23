namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
///     Request model for updating a role
/// </summary>
public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}