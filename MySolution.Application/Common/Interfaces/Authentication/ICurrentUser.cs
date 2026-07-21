namespace MySolution.Application.Common.Interfaces.Authentication;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Username { get; } 
    string Email { get;}
    IReadOnlyCollection<string> Roles { get; }
    /*IReadOnlyCollection<string> Permissions { get; }*/
    bool IsAuthenticated { get; }
    string? Jti {get;}
    DateTime? ExpiredAt { get; }
}