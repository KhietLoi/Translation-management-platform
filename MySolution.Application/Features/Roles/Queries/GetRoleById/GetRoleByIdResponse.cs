using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Roles.Queries.GetRoleById;

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