namespace MySolution.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Username { get; } 
    string Email { get;}
    bool IsActive { get;}
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Permissions { get; }
    bool IsAuthenticated { get; }
}