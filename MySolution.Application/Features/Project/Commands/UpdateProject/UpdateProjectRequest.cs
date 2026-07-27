namespace MySolution.Application.Features.Project.Commands.UpdateProject;

public class UpdateProjectRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}