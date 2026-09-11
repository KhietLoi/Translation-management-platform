using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectNamespace;

public class UpdateProjectNamespaceResponse : BaseResponse <UpdateProjectNamespaceData>
{

}

public class UpdateProjectNamespaceData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } =   string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}