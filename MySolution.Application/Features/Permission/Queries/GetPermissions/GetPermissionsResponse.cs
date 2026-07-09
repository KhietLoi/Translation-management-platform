using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Permission.Queries.GetPermissions;

public class GetPermissionsResponse : BaseResponse <GetPermissionsResult>
{
}

public class GetPermissionsResult
{
    public List<GetPermissionsData> Roles { get; set; } = [];
    public PagingInfo Paging { get; set; } = new();
}
public class GetPermissionsData
{
    public Guid Id { get; set; }
    public string Code { get; set; }  = string.Empty;
    public string? Description { get; set; }
}