using MediatR;

namespace MySolution.Application.Features.Project.Commands.CreateProjectFull;

public class CreateProjectFullCommand : IRequest<CreateProjectFullResponse>
{
    public CreateProjectFullRequest Payload { get; set; }

    public CreateProjectFullCommand(CreateProjectFullRequest payload)
    {
        Payload = payload;
    }
}