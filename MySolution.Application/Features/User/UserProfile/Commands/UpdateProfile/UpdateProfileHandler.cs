using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResponse>
{
    private readonly ILogger<UpdateProfileHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler
    (
        ILogger<UpdateProfileHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateProfileCommand, UpdateProfileResponse>

    public async Task<UpdateProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(UpdateProfileHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateProfileResponse();

        try
        {

            response.Ok();
        }
        catch (Exception exception)
        {
            exception.LogError(_logger, functionName);
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}