

using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Queries.GetProjectNamspaces;

public class GetProjectNamspacesResponse : BaseResponse <GetProjectNamespacesResult>
{
}

public class GetProjectNamespacesResult
{
    public List<GetProjectNamespaceData> Namespaces { get; set; } = [];
}

public class GetProjectNamespaceData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
}