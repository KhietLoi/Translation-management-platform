namespace MySolution.Application.Features.Project.Commands.UpdateProjectNamespace;

public class UpdateProjectNamespaceRequest
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
}