namespace MySolution.Application.Features.Project.Commands.CreateProjectNamespace;

public class CreateProjectNamespaceRequest
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } =  string.Empty;
}   