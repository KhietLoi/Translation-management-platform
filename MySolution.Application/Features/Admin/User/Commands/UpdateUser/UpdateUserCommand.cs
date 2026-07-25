using MediatR;

namespace MySolution.Application.Features.Admin.User.Commands.UpdateUser;

/// <summary>
///     Command to update a user
/// </summary>
public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public UpdateUserCommand(Guid userId, UpdateUserRequest payload)
    {
        Id = userId;
        Payload = payload;
    }

    public Guid Id { get; set; }
    public UpdateUserRequest Payload { get; }
}