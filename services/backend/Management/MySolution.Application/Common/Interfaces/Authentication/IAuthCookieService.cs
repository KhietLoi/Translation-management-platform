namespace MySolution.Application.Common.Interfaces.Authentication;

public interface IAuthCookieService
{
    void SetRefreshToken(string refreshToken);
    void RemoveRefreshToken();
    string?  GetRefreshToken();
}