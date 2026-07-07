using MySolution.Application.Common.Model;

namespace MySolution.Application.Features.Auth.RefreshToken;

public class RefreshTokenResponse : BaseResponse
{
    public string AccessToken { get; set; } =  string.Empty;
    public string RefreshToken { get; set; } =  string.Empty;
    public DateTime ExpiredAt { get; set; }
}