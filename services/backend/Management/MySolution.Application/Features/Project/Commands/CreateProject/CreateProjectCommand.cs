using MediatR;

namespace MySolution.Application.Features.Project.Commands.CreateProject;

public class CreateProjectCommand : IRequest<CreateProjectResponse>
{
    public CreateProjectRequest Payload { get; set; }

    public CreateProjectCommand(CreateProjectRequest payload)
    {
        Payload = payload;
    }
}