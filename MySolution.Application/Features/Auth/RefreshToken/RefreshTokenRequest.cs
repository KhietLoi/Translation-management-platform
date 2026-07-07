namespace MySolution.Application.Features.Auth.RefreshToken;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } =  string.Empty;
}