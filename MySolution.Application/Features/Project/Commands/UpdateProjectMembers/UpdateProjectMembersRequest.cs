using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectMembers;

public class UpdateProjectMembersRequest
{
    public List<ProjectMemberItem> Members { get; set; } = [];
}

public class ProjectMemberItem
{
    public Guid UserId { get; set; }
    public ProjectRole Role { get; set; }
}