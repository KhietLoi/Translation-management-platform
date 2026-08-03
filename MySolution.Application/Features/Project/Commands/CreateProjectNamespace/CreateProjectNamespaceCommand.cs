using MediatR;

namespace MySolution.Application.Features.Project.Commands.CreateProjectNamespace;

public class CreateProjectNamespaceCommand : IRequest<CreateProjectNamespaceResponse>
{
    public CreateProjectNamespaceRequest Payload { get; set; }
    public Guid ProjectId { get; set; }

    public CreateProjectNamespaceCommand(CreateProjectNamespaceRequest payload,  Guid projectId)
    {
        Payload = payload;
        ProjectId = projectId;
    }
}