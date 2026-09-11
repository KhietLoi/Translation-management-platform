using MediatR;

namespace MySolution.Application.Features.Project.Queries.GetProjectById;

public class GetProjectByIdQuery : IRequest<GetProjectByIdResponse>
{
    public Guid ProjectId { get; set; }

    public GetProjectByIdQuery(Guid projectId)
    {
        ProjectId = projectId;
    }
}