using MySolution.Application.Common.Models;
namespace MySolution.Application.Features.Auth.Login;

/// <summary>
/// Response for login operation
/// </summary>
public class LoginResponse : BaseResponse<LoginResult>
{
}
public class LoginResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAtAccessToken { get; set; }
}