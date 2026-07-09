using MediatR;

namespace MySolution.Application.Features.User.Commands.UpdateUser;

/// <summary>
/// Command to update a user
/// </summary>
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