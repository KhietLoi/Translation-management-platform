using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.CreateProject;

public class CreateProjectResponse : BaseResponse <CreateProjectData>
{
}
public class CreateProjectData
{
    public Guid ProjectId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } =  true;
    public DateTime CreatedAt { get; set; } 
}