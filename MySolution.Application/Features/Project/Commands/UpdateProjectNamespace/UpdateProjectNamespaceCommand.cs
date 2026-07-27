using MediatR;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectNamespace;

public class UpdateProjectNamespaceCommand : IRequest<UpdateProjectNamespaceResponse>
{
    public UpdateProjectNamespaceRequest Payload { get; set; }
    public Guid Id { get; set; }
    public UpdateProjectNamespaceCommand(UpdateProjectNamespaceRequest payload, Guid id)
    {
        Payload = payload;
        Id = id;
    }
}