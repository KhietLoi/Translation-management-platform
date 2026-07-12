namespace MySolution.Application.Features.User.Commands.UpdateUser;

/// <summary>
/// Request model for updating a user
/// </summary>
public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    //public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } 
    public List<Guid> RoleIds { get; set; } = [];
}