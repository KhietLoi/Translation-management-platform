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
    
    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RegisterCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Login a user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new LoginCommand(request),
            cancellationToken);

        if (!response.Success)
        {
            return ResponseHelper.ToResponse(
                response.StatusCode,
                response);
        }

        Response.Cookies.Append(
            "refreshToken",
            response.Data.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // localhost
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response,
            new
            {
                AccessToken = response.Data.AccessToken,
                ExpiredAt = response.Data.ExpiresAt
            });
    }

    /// <summary>
    /// Refresh the access token
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /*[HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RefreshTokenCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }*/
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        Console.WriteLine(refreshToken);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized("Refresh token is missing.");
        }

        var response = await mediator.Send(
            new RefreshTokenCommand(
                new RefreshTokenRequest
                {
                    RefreshToken = refreshToken
                }),
            cancellationToken);

        if (!response.Success)
        {
            Response.Cookies.Delete("refreshToken");

            return ResponseHelper.ToResponse(
                response.StatusCode,
                response);
        }

        Response.Cookies.Append(
            "refreshToken",
            response.Data.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // localhost
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response,
            new
            {
                AccessToken = response.Data.AccessToken,
                ExpiredAt = response.Data.ExpiredAt
            });
    }

    /// <summary>
    /// Logout a user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LogoutCommand(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
}