using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Application.Command.UpdateApplication;

public class UpdateApplicationHandler : IRequestHandler<UpdateApplicationCommand, UpdateApplicationResponse>
{
    private readonly ILogger<UpdateApplicationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateApplicationHandler
    (
        ILogger<UpdateApplicationHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in UpdateApplicationCommand, UpdateApplicationResponse>

    public async Task<UpdateApplicationResponse> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateApplicationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateApplicationResponse();

        try
        {
            var application = await _unitOfWork.Application
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.ApplicationId, cancellationToken);
            if (application is null)
            {
                _logger.LogInformation(functionName + "Application not found.");
                response.ErrorMessage = "Application not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var isNameApplicationExists = await _unitOfWork.Application
                .GetAll()
                .AsNoTracking()
                .AnyAsync(
                    x => x.Name == payload.Name &&
                         x.Id != request.ApplicationId,
                    cancellationToken);
            if (isNameApplicationExists)
            {
                _logger.LogInformation(functionName + "Application already exists.");
                
                response.ErrorMessage = "Application already exists.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            //Update data:
            var now = DateTime.UtcNow;
            application.IsActive = payload.IsActive;
            application.Description = payload.Description;
            application.Name = payload.Name;
            application.UpdatedAt = now;
            application.UpdatedBy = _currentUser.UserId;
            
            response.Data = new UpdateApplicationData
            {
                Id = application.Id,
                Name = application.Name,
                Description = application.Description,
                IsActive = application.IsActive,
                UpdatedAt = now,
                UpdatedBy = application.UpdatedBy
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}