using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Templates;
using MySolution.Application.Constants;

namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailHandler : IRequestHandler<ResendVerificationEmailCommand, ResendVerificationEmailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResendVerificationEmailHandler> _logger;
    
    public ResendVerificationEmailHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<ResendVerificationEmailHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task<ResendVerificationEmailResponse> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(ResendVerificationEmailHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ResendVerificationEmailResponse
        {
            Success = false,
            StatusCode = System.Net.HttpStatusCode.InternalServerError
        };

        try
        {
            // Find user by email
            var user = await _unitOfWork.User.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                _logger.LogWarning("{FunctionName} User not found: {Email}", functionName, payload.Email);
                response.ErrorMessage = "User not found.";
                response.WithStatus(System.Net.HttpStatusCode.NotFound);
                return response;
            }

            // Check if the user is already verified
            if (user.IsEmailVerified)
            {
                response.ErrorMessage = "User is already verified.";
                response.WithStatus(System.Net.HttpStatusCode.BadRequest);
                return response;
            }

            // Check if there are any active tokens for the user
            var oldTokens = await _unitOfWork.EmailVerificationToken.GetActiveTokensByUserId(user.Id);

            foreach (var oldToken in oldTokens)
            {
                oldToken.IsUsed = true;
            }

            var token = Guid.NewGuid().ToString("N");

            await _unitOfWork.EmailVerificationToken.Add(new Domain.Entities.EmailVerificationToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(AuthConstants.EmailVerificationExpiryHours),
                IsUsed = false
            });

            await _unitOfWork.SaveAsync(cancellationToken);
            var verifyUrl = $"http://localhost:5173/verify-email?token={token}";

            var html = EmailTemplateVerifyRegister.VerifyEmail(user.Username, verifyUrl,
                AuthConstants.EmailVerificationExpiryHours);
            await _emailService.SendEmailAsync(user.Email, "Verify your email", html, cancellationToken);

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
}