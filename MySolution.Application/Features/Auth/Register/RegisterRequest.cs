namespace MySolution.Application.Features.Auth.Register;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } =  string.Empty;
}