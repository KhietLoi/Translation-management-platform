using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectMembers;

public class UpdateProjectMembersResponse : BaseResponse <UpdateProjectMembersResult>
{
}

public class UpdateProjectMembersResult
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<UpdateProjectMembersData> Members { get; set; } = [];
}
public class UpdateProjectMembersData
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ProjectRole Role { get; set; }
}