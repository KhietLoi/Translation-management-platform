using MediatR;

namespace MySolution.Application.Features.User.Commands.DeleteUser;

/// <summary>
///     Command to delete a user by its ID.
/// </summary>
/// <param name="id"></param>
public class DeleteUserCommand(Guid id) : IRequest<DeleteUserResponse>
{
    public Guid Id { get; set; } = id;
}