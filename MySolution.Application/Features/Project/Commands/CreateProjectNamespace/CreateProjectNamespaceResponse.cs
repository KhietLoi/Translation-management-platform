using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.CreateProjectNamespace;

public class CreateProjectNamespaceResponse : BaseResponse<CreateProjectNamespaceData>
{
}

public class CreateProjectNamespaceData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } =   string.Empty;
    public DateTime CreatedAt { get; set; }
}