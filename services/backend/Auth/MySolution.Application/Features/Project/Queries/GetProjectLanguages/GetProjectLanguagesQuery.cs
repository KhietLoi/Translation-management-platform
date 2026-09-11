using MediatR;

namespace MySolution.Application.Features.Project.Queries.GetProjectLanguages;

public class GetProjectLanguagesQuery : IRequest<GetProjectLanguagesResponse>
{
    public Guid ProjectId { get; set; }

    public GetProjectLanguagesQuery(Guid projectId)
    {
        ProjectId = projectId;
    }
}