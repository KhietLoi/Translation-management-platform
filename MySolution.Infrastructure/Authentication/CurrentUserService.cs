using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Infrastructure.Authentication;

/// <summary>
/// Service to get the current authenticated user information from the HTTP context.
/// </summary>
/// <param name="httpContextAccessor"></param>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
    public Guid UserId
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }
    
    public string Username => User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    
    public string Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public IReadOnlyCollection<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList() ?? [];
    
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string Jti => User?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    public DateTime? ExpiredAt
    {
        get
        {
            var exp = User?.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            if (string.IsNullOrWhiteSpace(exp))
            {
                return null;
            }

            if (!long.TryParse(exp, out var unixTime))
            {
                return null;
            }

            return DateTimeOffset
                .FromUnixTimeSeconds(unixTime)
                .UtcDateTime;
        }
    }
        
}