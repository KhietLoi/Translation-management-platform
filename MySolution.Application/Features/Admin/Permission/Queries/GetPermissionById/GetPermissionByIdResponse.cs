using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Permission.Queries.GetPermissionById;

/// <summary>
///     Response class for the get permission by ID operation.
/// </summary>
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