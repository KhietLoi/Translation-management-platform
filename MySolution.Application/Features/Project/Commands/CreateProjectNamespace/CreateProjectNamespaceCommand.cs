using MediatR;

namespace MySolution.Application.Features.Project.Commands.CreateProjectNamespace;

public class CreateProjectNamespaceCommand : IRequest<CreateProjectNamespaceResponse>
{
    public CreateProjectNamespaceRequest Payload { get; set; }

    public CreateProjectNamespaceCommand(CreateProjectNamespaceRequest payload)
    {
        Payload = payload;
    }
}