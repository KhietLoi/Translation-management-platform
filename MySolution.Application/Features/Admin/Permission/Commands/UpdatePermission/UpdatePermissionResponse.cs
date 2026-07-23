using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Permission.Commands.UpdatePermission;

/// <summary>
///     Response class for the update permission operation.
/// </summary>
public class UpdatePermissionResponse : BaseResponse<UpdatePermissionData>
{
}

public class UpdatePermissionData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}