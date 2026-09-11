using MediatR;

namespace MySolution.Application.Features.Project.Commands.DeleteProject;

public class DeleteProjectCommand : IRequest<DeleteProjectResponse>
{
    public Guid ProjectId { get; }

    public DeleteProjectCommand(Guid projectId)
    {
        ProjectId = projectId;
    }
}