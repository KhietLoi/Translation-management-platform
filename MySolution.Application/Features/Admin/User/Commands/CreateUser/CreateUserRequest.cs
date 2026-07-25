namespace MySolution.Application.Features.Admin.User.Commands.CreateUser;

/// <summary>
///     Request model for creating a new user
/// </summary>
public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // public string Password { get; set; } = string.Empty;
    public List<Guid> RoleIds { get; set; } = [];
}