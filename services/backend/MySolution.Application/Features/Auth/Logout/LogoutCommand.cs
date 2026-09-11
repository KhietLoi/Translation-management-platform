using MediatR;

namespace MySolution.Application.Features.Auth.Logout;

/// <summary>
///     Command to logout a user
/// </summary>
public class LogoutCommand : IRequest<LogoutResponse>
{
}