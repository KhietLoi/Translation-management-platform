using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.User.Commands.DeleteUser;

/// <summary>
///     Response class for the delete user operation.
/// </summary>
public class DeleteUserResponse : BaseResponse<DeleteUserData>
{
}

public class DeleteUserData
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}