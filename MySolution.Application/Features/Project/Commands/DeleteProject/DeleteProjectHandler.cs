using System.Net;
using MediatR;
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
            var project = await _unitOfWork.Project.GetByIdAsync(request.ProjectId);
            if (project == null)
            {
                response.ErrorMessage = "Project not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (project.IsActive)
            {
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