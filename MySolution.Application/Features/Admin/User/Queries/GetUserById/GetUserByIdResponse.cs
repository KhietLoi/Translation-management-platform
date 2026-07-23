using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.Queries.GetUserById;

public class GetUserByIdResponse : BaseResponse<GetUserByIdData>
{
}

public class GetUserByIdData
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RoleData> Roles { get; set; } = [];
    public List<PermissionData> Permissions { get; set; } = [];
}

public class RoleData
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class PermissionData
{
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string? PermissionDescription { get; set; }
}