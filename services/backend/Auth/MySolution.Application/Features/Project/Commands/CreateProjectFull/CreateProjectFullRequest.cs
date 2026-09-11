namespace MySolution.Application.Features.Project.Commands.CreateProjectFull;

public class CreateProjectFullRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> LanguageIds { get; set; } = new();
    public List<Guid> MemberIds { get; set; } = new();
    public List<CreateNamespaceRequest> Namespaces { get; set; } = new();
}

public class CreateNamespaceRequest
{
    public string Name { get; set; } = string.Empty;
}