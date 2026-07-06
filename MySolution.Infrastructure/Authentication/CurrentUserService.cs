using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Authentication;

public class CurrentUserService:ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse((ReadOnlySpan<byte>)value, out var id)
                ? id
                : Guid.Empty;

        }
    } 
    public string Username { get; }
    public string Email { get; }
    public bool IsActive { get; }
    public IReadOnlyCollection<string> Roles { get; }
    public IReadOnlyCollection<string> Permissions { get; }
    public bool IsAuthenticated { get; }
    
}