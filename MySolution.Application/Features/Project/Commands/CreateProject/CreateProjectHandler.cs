using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Project.Commands.CreateProject;

public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, CreateProjectResponse>
{
    private readonly ILogger<CreateProjectHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateProjectHandler
    (
        ILogger<CreateProjectHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateProjectCommand, CreateProjectResponse>

    public async Task<CreateProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateProjectHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateProjectResponse();

        try
        {
            // Check if project name already exists
            var isNameExists = await _unitOfWork.Project.ExistsByNameAsync(payload.Name);
            if (isNameExists)
            {
                response.ErrorMessage = $"Project with name {payload.Name} already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }

            var project = new Domain.Entities.Project
            {
                Id = Guid.CreateVersion7(),
                Name = payload.Name,
                Description = payload.Description,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Project.Add(project);
            await _unitOfWork.SaveAsync(cancellationToken);
            response.Data = new CreateProjectData
            {
                ProjectId = project.Id,
                Name = project.Name,
                Description = project.Description,
                IsActive = project.IsActive,
                CreatedAt = project.CreatedAt
            };
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
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