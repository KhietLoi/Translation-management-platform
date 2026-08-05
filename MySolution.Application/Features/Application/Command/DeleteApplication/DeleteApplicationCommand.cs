using MediatR;

namespace MySolution.Application.Features.Application.Command.DeleteApplication;

public class DeleteApplicationCommand : IRequest<DeleteApplicationResponse>
{
    public Guid Id { get; set; }
    public DeleteApplicationCommand(Guid id)
    {
        Id = id;
    }
}