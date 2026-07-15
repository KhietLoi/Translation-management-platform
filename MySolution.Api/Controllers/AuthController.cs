using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Auth.ChangePassword;
using MySolution.Application.Features.Auth.ForgotPassword;
using MySolution.Application.Features.Auth.Login;
using MySolution.Application.Features.Auth.Logout;
using MySolution.Application.Features.Auth.RefreshToken;
using MySolution.Application.Features.Auth.Register;
using MySolution.Application.Features.Auth.ResendVerificationEmail;
using MySolution.Application.Features.Auth.ResetPassword;
using MySolution.Application.Features.Auth.VerifyEmail;

using LoginRequest = MySolution.Application.Features.Auth.Login.LoginRequest;
using RegisterRequest = MySolution.Application.Features.Auth.Register.RegisterRequest;

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
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LoginCommand(request), cancellationToken);

        if (!response.Success)
        {
            return ResponseHelper.ToResponse(response.StatusCode, response);
        }
        Response.Cookies.Append("refreshToken", response.Data.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false, 
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        Console.WriteLine(refreshToken);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized("Refresh token is missing.");
        }

        var response = await mediator.Send( new RefreshTokenCommand( new RefreshTokenRequest { RefreshToken = refreshToken }), cancellationToken);

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

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest request,  CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ChangePasswordCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new VerifyEmailCommand(token), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    [HttpPost("resend-verify-email")]
    public async Task<IActionResult> ResendVerifyEmail([FromQuery] ResendVerificationEmailRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ResendVerificationEmailCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
   
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ForgotPasswordCommand(request),cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ResetPasswordCommand(request),  cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
}