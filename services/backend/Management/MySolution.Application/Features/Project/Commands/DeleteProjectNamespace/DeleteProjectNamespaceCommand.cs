using MediatR;

namespace MySolution.Application.Features.Project.Commands.DeleteProjectNamespace;

public class DeleteProjectNamespaceCommand : IRequest<DeleteProjectNamespaceResponse>
{
    public Guid Id { get; set; }

    public DeleteProjectNamespaceCommand(Guid id)
    {
        Id = id;
    }
}