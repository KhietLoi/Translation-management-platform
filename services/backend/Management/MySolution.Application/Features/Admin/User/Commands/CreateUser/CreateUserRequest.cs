namespace MySolution.Application.Features.Admin.User.Commands.CreateUser;


public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Guid>? RoleIds { get; set; } = [];
}