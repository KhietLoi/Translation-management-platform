using MediatR;
namespace MySolution.Application.Features.Users.Commands.CreateUser;
public class CreateUserCommand : IRequest <CreateUserResponse>
{
    public CreateUserRequest Payload { get; }

    public CreateUserCommand(CreateUserRequest payload)
    {
        Payload = payload;
    }
}