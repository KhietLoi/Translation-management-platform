using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Project.Commands.UpdateProject;

public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, UpdateProjectResponse>
{
    private readonly ILogger<UpdateProjectHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectHandler
    (
        ILogger<UpdateProjectHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateProjectCommand, UpdateProjectResponse>

    public async Task<UpdateProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateProjectHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateProjectResponse();

        try
        {
            var project = await _unitOfWork.Project
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (project == null)
            {
                _logger.LogInformation("{FunctionName} Project with id {ProjectId} not found", functionName, request.Id);
                
                response.ErrorMessage =  $"Project with id {request.Id} not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            // Check if project name already exists
            var isNameExists = await _unitOfWork.Project.ExistsByNameAsync(payload.Name, request.Id);
            if (isNameExists)
            {
                _logger.LogInformation("{FunctionName} Project with name {ProjectName} already exists", functionName, payload.Name);
                
                response.ErrorMessage =  $"Project with id {request.Id} is already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            //Update
            project.Name = payload.Name;
            project.Description = payload.Description;
            project.UpdatedAt = DateTime.UtcNow;
            project.IsActive  = payload.IsActive;
            
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new UpdateProjectData
            {
                ProjectId = project.Id,
                Name = payload.Name,
                Description = payload.Description,
                IsActive = payload.IsActive,
                CreatedAt = project.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("{FunctionName} Project updated successfully", functionName);
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