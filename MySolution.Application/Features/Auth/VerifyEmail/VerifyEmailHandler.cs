using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.VerifyEmail;

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResponse>
{

    private readonly ILogger<VerifyEmailHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailVerificationTokenService _emailVerificationTokenService;

    public VerifyEmailHandler
    (
        ILogger<VerifyEmailHandler> logger,
        IUnitOfWork unitOfWork,
        IEmailVerificationTokenService emailVerificationTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _emailVerificationTokenService = emailVerificationTokenService;
    }

    public async Task<VerifyEmailResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(VerifyEmailHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new VerifyEmailResponse();

        try
        {
            var payload = _emailVerificationTokenService.ValidateToken(request.Token);
            if (payload.ExpiredAt < DateTime.UtcNow)
            {
                response.ErrorMessage = "Verification token is expired.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var user = _unitOfWork.User.GetByIdAsync(payload.UserId).Result;
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (user.IsEmailVerified)
            {
                response.ErrorMessage = "Email is already verified.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            user.IsEmailVerified = true;
            await _unitOfWork.SaveAsync(cancellationToken);
         
            response.Data = new VerifyEmailData
            {
                Email = user.Email
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }
        
        return response;
    }
}