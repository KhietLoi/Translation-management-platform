using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Project.Queries.GetProjectMembers;

public class GetProjectMembersResponse : BaseResponse <GetProjectMembersResult>
{

}

public class GetProjectMembersResult
{
    public List<GetProjectMemberData> Members { get; set; } = [];
}

public class GetProjectMemberData
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}