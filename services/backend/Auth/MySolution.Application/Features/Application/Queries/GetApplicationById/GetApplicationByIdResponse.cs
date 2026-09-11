using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Application.Queries.GetApplicationById;

public class GetApplicationByIdResponse : BaseResponse <GetApplicationByIdData>
{
}

public class GetApplicationByIdData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsActive { get; set; }
}