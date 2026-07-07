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
public class AuthController : Controller
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new RegisterCommand(request), cancellationToken);

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response,
            response.Data
           );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new LoginCommand(request),
            cancellationToken);

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response,
            response.Data);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new LogoutCommand(),
            cancellationToken);

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response);
    }
    
}