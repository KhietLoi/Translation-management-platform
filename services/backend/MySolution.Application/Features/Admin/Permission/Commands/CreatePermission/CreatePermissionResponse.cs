using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Admin.Permission.Commands.CreatePermission;

/// <summary>
///     Response class for the create permission operation.
/// </summary>
public class CreatePermissionResponse : BaseResponse<CreatePermissionData>
{
}

public class CreatePermissionData
{
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}