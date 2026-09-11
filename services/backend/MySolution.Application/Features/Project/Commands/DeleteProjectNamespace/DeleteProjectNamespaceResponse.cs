using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.DeleteProjectNamespace;

public class DeleteProjectNamespaceResponse : BaseResponse <DeleteProjectNamespaceData>
{
}

public class DeleteProjectNamespaceData
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } =   string.Empty;
}