using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Queries.GetProjects;

public class GetProjectsResponse : BaseResponse <GetProjectsResult>
{
}

public class GetProjectsResult
{ 
    public List<GetProjectData>? Projects { get; set; }
}

public class GetProjectData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int LanguageCount { get; set; }
    public int MemberCount { get; set; }
    public int NamespaceCount { get; set; }
    public int TotalTranslationCount { get; set; }
    public int CompletedTranslationCount { get; set; }
    public decimal ProgressPercentage { get; set; }
    public List<ProjectLanguageItem> Languages { get; set; } = new();
    public List<ProjectMemberItem> Members { get; set; } = new();
}

public class ProjectLanguageItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class ProjectMemberItem
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}