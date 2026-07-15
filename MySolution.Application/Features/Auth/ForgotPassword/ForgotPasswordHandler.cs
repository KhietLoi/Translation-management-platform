using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;
using MySolution.Domain.Entities;
namespace MySolution.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly ILogger<ForgotPasswordHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public ForgotPasswordHandler
    (
        ILogger<ForgotPasswordHandler> logger,
		IUnitOfWork unitOfWork,
        IEmailService emailService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    #region Implementation of IRequestHandler<in ForgotPasswordCommand, ForgotPasswordResponse>

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(ForgotPasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ForgotPasswordResponse();

        try
        {
            var user = await _unitOfWork.User.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Disable old Token
            var oldTokens = await _unitOfWork.PasswordResetToken.GetActiveTokensByUserIdAsync(user.Id);
            foreach (var oldToken in oldTokens)
            {
                oldToken.IsUsed = true;
            }
            
            var resetToken = Guid.CreateVersion7().ToString("N");
            await _unitOfWork.PasswordResetToken.Add(new PasswordResetToken
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Token = resetToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(AuthConstants.PasswordResetExpiryHours),
                IsUsed = false
            });
            await _unitOfWork.SaveAsync(cancellationToken);
            //URL Reset:
            var resetUrl = $"https://localhost:5173/reset/{resetToken}";
            var html = ResetPasswordTemplate.ResetPassword(user.Username, resetUrl, AuthConstants.PasswordResetExpiryHours);
            //SendEmail
            await _emailService.SendEmailAsync(user.Email,"Reset Password",html, cancellationToken);
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResendVerificationEmail failed");
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}