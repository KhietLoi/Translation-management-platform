using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Constants;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.Authentication;

public class AuthCookieService : IAuthCookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;

    public AuthCookieService
    (
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
    }
    public void SetRefreshToken(string refreshToken)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append
        (
            AuthConstants.RefreshTokenCookieName,
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
            }
        );
    }

    public void RemoveRefreshToken()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(AuthConstants.RefreshTokenCookieName);
    }

    public string? GetRefreshToken()
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.RefreshTokenCookieName];
    }
}