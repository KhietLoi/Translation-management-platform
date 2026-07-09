using MediatR;

namespace MySolution.Application.Features.Auth.Register;

/// <summary>
/// RegisterCommand is a command that represents a request to register a new user.
/// </summary>
public class RegisterCommand : IRequest<RegisterResponse>
{
    public RegisterRequest Payload { get; }
    public RegisterCommand(RegisterRequest payload)
    {
        Payload = payload;
    }
}