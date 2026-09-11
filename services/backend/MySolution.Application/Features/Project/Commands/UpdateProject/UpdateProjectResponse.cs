using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.UpdateProject;

public class UpdateProjectResponse : BaseResponse <UpdateProjectData>
{

}

public class UpdateProjectData
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}