namespace MySolution.Application.Features.Project.Commands.CreateProject;

public class CreateProjectRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}