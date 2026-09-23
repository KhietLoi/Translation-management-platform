namespace MySolution.Application.Features.Admin.Permission.Commands.UpdatePermission;

public class UpdatePermissionRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}