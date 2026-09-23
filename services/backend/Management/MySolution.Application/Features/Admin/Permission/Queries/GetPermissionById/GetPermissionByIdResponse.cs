using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Admin.Permission.Queries.GetPermissionById;

public class GetPermissionByIdResponse : BaseResponse<GetPermissionByIdData>
{
}

public class GetPermissionByIdData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}