using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Admin.User.Commands.CreateUser;

/// <summary>
///     Response class for the create user operation.
/// </summary>
public class CreateUserResponse : BaseResponse<CreateUserData>
{
}

public class CreateUserData
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = true;
    public UserStatus Status { get; set; }
}