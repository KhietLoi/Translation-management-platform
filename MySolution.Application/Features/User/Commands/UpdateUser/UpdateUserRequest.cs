namespace MySolution.Application.Features.User.Commands.UpdateUser;

public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } //Update after
    public List<Guid> RoleIds { get; set; } = [];
}