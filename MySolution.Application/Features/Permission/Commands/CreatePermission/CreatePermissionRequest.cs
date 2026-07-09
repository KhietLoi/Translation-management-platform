namespace MySolution.Application.Features.Permission.Commands.CreatePermission;

/// <summary>
/// Represents a request to create a new permission with a unique code and an optional description.
/// </summary>
public class CreatePermissionRequest
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}