using MediatR;

namespace MySolution.Application.Features.Auth.Login;

/// <summary>
/// LoginCommand is a command that represents a request to log in a user.
/// It contains the login request payload and implements the IRequest interface from MediatR,
/// which allows it to be handled by a corresponding handler that processes the login logic and returns a LoginResponse.
/// </summary>
/// <param name="payload"></param>
public class LoginCommand(LoginRequest payload) : IRequest<LoginResponse>
{
    public LoginRequest Payload { get; } = payload;
}