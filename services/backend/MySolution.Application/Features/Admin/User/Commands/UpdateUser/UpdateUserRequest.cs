using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Admin.User.Commands.UpdateUser;

/// <summary>
///     Request model for updating a user
/// </summary>
public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public List<Guid> RoleIds { get; set; } = [];
}