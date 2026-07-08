using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Auth.Login;
using MySolution.Application.Features.Auth.Logout;
using MySolution.Application.Features.Auth.RefreshToken;
using MySolution.Application.Features.Auth.Register;

namespace MySolution.Api.Controllers;

[Route("api/[controller]") ]
[ApiController]
public class AuthController(IMediator mediator) : Controller
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RegisterCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LoginCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LogoutCommand(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
}