using System.Security.Claims;
using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}