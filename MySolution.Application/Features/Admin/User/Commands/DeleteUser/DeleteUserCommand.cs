using MediatR;
using MySolution.Application.Features.User.Commands.DeleteUser;

namespace MySolution.Application.Features.Admin.User.Commands.DeleteUser;

/// <summary>
///     Command to delete a user by its ID.
/// </summary>
/// <param name="id"></param>
public class DeleteUserCommand(Guid id) : IRequest<DeleteUserResponse>
{
    public Guid Id { get; set; } = id;
}