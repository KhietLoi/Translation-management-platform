namespace MySolution.Application.Features.Admin.Permission.Commands.CreatePermission;

public class CreatePermissionRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}