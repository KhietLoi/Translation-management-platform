using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Application.Command.CreateApplication;

public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, CreateApplicationResponse>
{
    private readonly ILogger<CreateApplicationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateApplicationHandler
    (
        ILogger<CreateApplicationHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in CreateApplicationCommand, CreateApplicationResponse>

    public async Task<CreateApplicationResponse> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateApplicationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateApplicationResponse();

        try
        {
            //Check if application name is already exists
            var isNameUnique = await _unitOfWork.Application.IsApplicationNameExistsAsync(payload.Name);
            if (isNameUnique)
            {
                response.ErrorMessage = "Application already exists.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            //Check project exists
            var projectExists = await _unitOfWork.Project.ExistsAsync(payload.ProjectId);
            if (!projectExists)
            {
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Create Application
            DateTime now = DateTime.UtcNow;
            var application = new Domain.Entities.Application
            {
                Id = Guid.CreateVersion7(),
                ProjectId = payload.ProjectId,
                Name = payload.Name,
                Description = payload.Description,
                IsActive = true,
                CreatedAt = now,
                CreatedBy = _currentUser.UserId,
            };
            
            await _unitOfWork.Application.Add(application);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new CreateApplicationData
            {
                Id = application.Id,
                ProjectId = application.ProjectId,
                Name = application.Name,
                Description = application.Description,
                IsActive = application.IsActive,
                CreatedAt = now,
                CreatedBy = application.CreatedBy
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