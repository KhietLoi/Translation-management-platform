using MediatR;

namespace MySolution.Application.Features.Auth.Logout;

public class LogoutCommand : IRequest<LogoutResponse>
{
}