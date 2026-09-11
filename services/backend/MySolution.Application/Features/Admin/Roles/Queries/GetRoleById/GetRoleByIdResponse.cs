using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Admin.Roles.Queries.GetRoleById;

/// <summary>
///     Response class for the get role by ID operation.
/// </summary>
public class GetRoleByIdResponse : BaseResponse<GetRoleByIdData>
{
}

public class GetRoleByIdData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<GetRolePermissionData> Permissions { get; set; } = [];
}

public class GetRolePermissionData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}