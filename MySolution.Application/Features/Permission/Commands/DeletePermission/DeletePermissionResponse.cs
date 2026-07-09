using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Permission.Commands.DeletePermission;

public class DeletePermissionResponse : BaseResponse <DeletePermissionData>
{
    
}

public class DeletePermissionData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}