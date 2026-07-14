using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.VerifyEmail;

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResponse>
{

    private readonly ILogger<VerifyEmailHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailHandler(ILogger<VerifyEmailHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<VerifyEmailResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var functionName =
            $"{nameof(VerifyEmailHandler)} =>";

        _logger.LogInformation(functionName);

        var response = new VerifyEmailResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var token = await _unitOfWork.EmailVerificationToken.GetByTokenAsync(request.Token);
            if (token is null)
            {
                response.ErrorMessage = "Invalid verification token.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            if (token.IsUsed)
            {
                response.ErrorMessage = "Verification token already used.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            if (token.IsExpired)
            {
                response.ErrorMessage = "Verification token expired.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            token.User.IsEmailVerified = true;
            token.IsUsed = true;
            await _unitOfWork.SaveAsync(cancellationToken);

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