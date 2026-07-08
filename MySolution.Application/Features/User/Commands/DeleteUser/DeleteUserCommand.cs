using MediatR;

namespace MySolution.Application.Features.User.Commands.DeleteUser;

public class DeleteUserCommand(Guid id) : IRequest<DeleteUserResponse>
{
    public Guid Id { get; set; } = id;
}