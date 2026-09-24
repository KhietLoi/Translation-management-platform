using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MySolution.Api.Helpers;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Features.Auth.ChangePassword;
using MySolution.Application.Features.Auth.ForgotPassword;
using MySolution.Application.Features.Auth.GetCurrentUser;
using MySolution.Application.Features.Auth.Login;
using MySolution.Application.Features.Auth.Logout;
using MySolution.Application.Features.Auth.RefreshToken;
using MySolution.Application.Features.Auth.Register;
using MySolution.Application.Features.Auth.ResendVerificationEmail;
using MySolution.Application.Features.Auth.ResetPassword;
using MySolution.Application.Features.Auth.VerifyEmail;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator,  IAuthCookieService cookieService) : Controller
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [EnableRateLimiting("auth-register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RegisterCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [EnableRateLimiting("auth-login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LoginCommand(request), cancellationToken);
        if (!response.Success)
        {
            return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
        }

        if (response.Data != null)
        {
            cookieService.SetRefreshToken(response.Data.RefreshToken);
        }
        
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Refreshes the access token using the refresh token stored in cookies.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    [EnableRateLimiting("auth-refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = cookieService.GetRefreshToken();
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized("Refresh token is missing.");
        }

        var response = await mediator.Send(new RefreshTokenCommand(
            new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            }), cancellationToken);
        
        if (!response.Success)
        {
            cookieService.RemoveRefreshToken();
            return ResponseHelper.ToResponse(response.StatusCode, response);
        }

        if (response.Data != null)
        {
            cookieService.SetRefreshToken(response.Data.RefreshToken);
        }

        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Logs out the user by invalidating the refresh token and removing it from cookies.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new LogoutCommand(), cancellationToken);
        cookieService.RemoveRefreshToken();
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// Changes the password
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ChangePasswordCommand(request), cancellationToken);
        if (response.Success)
        {
            cookieService.RemoveRefreshToken();
        }
        
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// Verifies the user's email using the provided token.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("verify-email/{token}")]
    public async Task<IActionResult> VerifyEmail(string token, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new VerifyEmailCommand(token), cancellationToken);
        if (!response.Success)
        {
            return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
        }
        
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// Resends the verification email to the user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("resend-verify-email")]
    [EnableRateLimiting("auth-resend-verification-email")]
    public async Task<IActionResult> ResendVerifyEmail([FromBody] ResendVerificationEmailRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ResendVerificationEmailCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// Initiates the forgot password process by sending a password reset email to the user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth-forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ForgotPasswordCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// Resets the user's password using the provided token and new password.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ResetPasswordCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
    /// <summary>
    /// Retrieves the current authenticated user's information.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}