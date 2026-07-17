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
    private readonly IEmailVerificationTokenService  _tokenService;
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    
    public ResendVerificationEmailHandler
    (
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<ResendVerificationEmailHandler> logger,
        IEmailVerificationTokenService tokenService,
        IApplicationUrlProvider applicationUrlProvider
    )
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
        _tokenService = tokenService;
        _applicationUrlProvider = applicationUrlProvider;
    }
    
    public async Task<ResendVerificationEmailResponse> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var response = new ResendVerificationEmailResponse();

        try
        {
            var user = await _unitOfWork.User.GetByEmailAsync(request.Payload.Email);
            if (user is null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (user.IsEmailVerified)
            {
                response.ErrorMessage = "Email already verified.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var token = _tokenService.GenerateVerificationToken(user.Id, user.Email);
            var verifyUrl = _applicationUrlProvider.GetVerifyEmailUrl(Uri.EscapeDataString(token));
            var html =
                EmailTemplateVerifyRegister.VerifyEmail(
                    user.Username,
                    verifyUrl,
                    AuthConstants.EmailVerificationExpiryMinutes);

            await _emailService.SendEmailAsync(
                user.Email,
                "Verify your email",
                html,
                cancellationToken);
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