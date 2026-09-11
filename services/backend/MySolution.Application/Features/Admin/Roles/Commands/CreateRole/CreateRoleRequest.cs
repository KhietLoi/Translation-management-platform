namespace MySolution.Application.Features.Admin.Roles.Commands.CreateRole;

/// <summary>
///     Request model for creating a new role
/// </summary>
public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}