using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Permission.Queries.GetPermissionById;

public class GetPermissionByIdResponse : BaseResponse <GetPermissionByIdData>
{
}
public class GetPermissionByIdData
{
    public Guid Id { get; set; }
    public string Code { get; set; } = String.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
}