using MediatR;

namespace MySolution.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public Guid Id { get; set; }
    public UpdateUserRequest Payload { get; }
    public UpdateUserCommand(Guid userId, UpdateUserRequest payload)
    {
        Id = userId;
        Payload = payload;
    }
}