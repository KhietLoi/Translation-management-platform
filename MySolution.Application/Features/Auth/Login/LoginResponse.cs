using MySolution.Application.Common.Model;
namespace MySolution.Application.Features.Auth.Login;
public class LoginResponse : BaseResponse<LoginResult>
{
}
public class LoginResult
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}