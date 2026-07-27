using MediatR;

namespace MySolution.Application.Features.Project.Commands.UpdateProject;

public class UpdateProjectCommand : IRequest<UpdateProjectResponse>
{
    public UpdateProjectRequest Payload { get; set; }
    public Guid Id { get; set; }

    public UpdateProjectCommand(Guid id, UpdateProjectRequest payload)
    {
        Id = id;
        Payload = payload;
    }
}