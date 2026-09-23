namespace MySolution.Application.Features.Admin.Roles.Commands.CreateRole;


public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}