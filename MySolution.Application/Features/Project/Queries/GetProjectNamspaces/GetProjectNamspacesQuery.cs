using MediatR;

namespace MySolution.Application.Features.Project.Queries.GetProjectNamspaces;

public class GetProjectNamspacesQuery : IRequest<GetProjectNamspacesResponse>
{
    public Guid ProjectId { get; set; }

    public GetProjectNamspacesQuery(Guid projectId)
    {
        ProjectId = projectId;
    }
}