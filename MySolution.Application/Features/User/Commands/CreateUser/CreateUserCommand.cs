using MediatR;

namespace MySolution.Application.Features.User.Commands.CreateUser;
public class CreateUserCommand(CreateUserRequest payload) : IRequest<CreateUserResponse>
{
    public CreateUserRequest Payload { get; } = payload;
}