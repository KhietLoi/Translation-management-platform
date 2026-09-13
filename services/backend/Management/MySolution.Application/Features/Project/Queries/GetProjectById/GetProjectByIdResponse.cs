using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Project.Queries.GetProjectById;

public class GetProjectByIdResponse : BaseResponse <GetProjectIdData>
{
}
public class GetProjectIdData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int LanguageCount { get; set; }
    public int MemberCount { get; set; }
    public int NamespaceCount { get; set; }
}