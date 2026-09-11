using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Admin.Permission.Queries.GetPermissions;

/// <summary>
///     Response class for the get permissions operation.
/// </summary>
public class GetPermissionsResponse : BaseResponse<GetPermissionsResult>
{
}

public class GetPermissionsResult
{
    public List<GetPermissionsData> Permissions { get; set; } = [];
    public PagingInfo Paging { get; set; } = new();
}

public class GetPermissionsData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
}