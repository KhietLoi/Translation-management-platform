using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Permission.Commands.DeletePermission;

/// <summary>
/// Response class for the delete permission operation.
/// </summary>
public class DeletePermissionResponse : BaseResponse <DeletePermissionData>
{
}
public class DeletePermissionData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}