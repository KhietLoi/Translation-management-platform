using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Project.Commands.DeleteProject;

public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, DeleteProjectResponse>
{
    private readonly ILogger<DeleteProjectHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectHandler
    (
        ILogger<DeleteProjectHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteProjectCommand, DeleteProjectResponse>

    public async Task<DeleteProjectResponse> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteProjectHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteProjectResponse();

        try
        {
            var project = await _unitOfWork.Project
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
            
            if (project == null)
            {
                _logger.LogInformation("{FunctionName} Project with ID {ProjectId} not found.", functionName, request.ProjectId);
                
                response.ErrorMessage = "Project not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (project.IsActive)
            {
                _logger.LogInformation("{FunctionName} Project with ID {ProjectId} is active and cannot be deleted.", functionName, request.ProjectId);
                
                response.ErrorMessage = "Project is already active";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            _unitOfWork.Project.Delete(project);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new DeleteProjectData
            {
                ProjectId = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsActive = project.IsActive
            };
            
            _logger.LogInformation("{FunctionName} Project with ID {ProjectId} deleted successfully.", functionName, request.ProjectId);
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