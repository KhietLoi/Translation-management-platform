using MediatR;

namespace MySolution.Application.Features.Project.Queries.GetProjectMembers;

public class GetProjectMembersQuery : IRequest<GetProjectMembersResponse>
{
    public Guid ProjectId { get; set; }

    public GetProjectMembersQuery(Guid projectId)
    {
        ProjectId = projectId;
    }
}