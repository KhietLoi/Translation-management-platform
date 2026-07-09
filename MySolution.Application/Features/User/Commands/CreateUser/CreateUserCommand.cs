using MediatR;

namespace MySolution.Application.Features.User.Commands.CreateUser;

/// <summary>
/// Command to create a new user
/// </summary>
/// <param name="payload"></param>
public class CreateUserCommand(CreateUserRequest payload) : IRequest<CreateUserResponse>
{
    public CreateUserRequest Payload { get; } = payload;
}