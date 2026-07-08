namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}