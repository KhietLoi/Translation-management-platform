using System.Security.Claims;
using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Authentication;

public interface IJwtService
{
    string GenerateJwtToken(User user, string jti);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    DateTime GetAccessTokenExpirationDate();
    DateTime GetRefreshTokenExpirationDate();
}