namespace MySolution.Application.Features.Permission.Commands.UpdatePermission;

/// <summary>
///     Request model for updating a permission
/// </summary>
public class UpdatePermissionRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}