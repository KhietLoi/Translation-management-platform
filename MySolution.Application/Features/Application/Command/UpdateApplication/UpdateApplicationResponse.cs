using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Application.Command.UpdateApplication;

public class UpdateApplicationResponse : BaseResponse <UpdateApplicationData>
{
}

public class UpdateApplicationData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}