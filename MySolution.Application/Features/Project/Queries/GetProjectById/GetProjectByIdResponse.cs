using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Project.Queries.GetProjectById;

public class GetProjectByIdResponse : BaseResponse <GetProjectIdData>
{
    //! Phan nay update sau co the tra them nhieu du lieu hon
    
}
public class GetProjectIdData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<GetProjectLanguageData> ProjectLanguages { get; set; } = [];
    public List<GetProjectMemberData> ProjectMembers { get; set; } = [];
    public List<GetProjectNamespaceData> ProjectNamespaces { get; set; } = [];
}
public class GetProjectLanguageData
{
    public Guid LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
public class GetProjectMemberData
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ProjectRole  Role { get; set; }
}
public class GetProjectNamespaceData
{
    public Guid NamespaceId { get; set; }
    public string Name  { get; set; } = string.Empty;
}