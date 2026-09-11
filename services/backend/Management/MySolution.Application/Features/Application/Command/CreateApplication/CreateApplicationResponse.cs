using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Application.Command.CreateApplication;

public class CreateApplicationResponse : BaseResponse <CreateApplicationData>
{
}

public class CreateApplicationData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}