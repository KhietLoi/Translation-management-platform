using MediatR;

namespace MySolution.Application.Features.User.Commands.DeleteUser;

public class DeleteUserCommand : IRequest <DeleteUserResponse>
{
    public Guid Id { get; set; }
    public DeleteUserCommand(Guid id)
    {
        Id = id;
    }
}