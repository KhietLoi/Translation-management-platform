using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Authentication;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;
    public Guid UserId
    {
        get
        {
            var value = User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            return Guid.TryParse(value, out var id)
                ? id
                : Guid.Empty;
        }
    }
    
    public string Username =>
        User?   
            .FindFirst(ClaimTypes.Name)?
            .Value
        ?? string.Empty;
    
    public string Email =>
        User? 
            .FindFirst(ClaimTypes.Email)?
            .Value
        ?? string.Empty;

    public IReadOnlyCollection<string> Roles =>
        User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList()
        ?? [];

    public IReadOnlyCollection<string> Permissions =>
        User?
            .FindAll("permission")
            .Select(x => x.Value)
            .ToList()
        ?? [];
    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;
    
}