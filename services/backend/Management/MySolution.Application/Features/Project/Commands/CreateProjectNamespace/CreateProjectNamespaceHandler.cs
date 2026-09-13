using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
namespace MySolution.Application.Features.Project.Commands.CreateProjectNamespace;

public class CreateProjectNamespaceHandler : IRequestHandler<CreateProjectNamespaceCommand, CreateProjectNamespaceResponse>
{
    private readonly ILogger<CreateProjectNamespaceHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateProjectNamespaceHandler
    (
        ILogger<CreateProjectNamespaceHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateProjectNamespaceCommand, CreateProjectNamespaceResponse>

    public async Task<CreateProjectNamespaceResponse> Handle(CreateProjectNamespaceCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateProjectNamespaceHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateProjectNamespaceResponse();

        try
        {
            //Check project
            var project =  await  _unitOfWork.Project
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
            
            if (project == null)
            {
                _logger.LogInformation("{FunctionName} Project not found. ProjectId: {ProjectId}", functionName, request.ProjectId);
                
                response.ErrorMessage = "Project not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check project namespace name is already exists
            var isNameExist = await _unitOfWork.Namespace.ExistsAsync(project.Id, payload.Name);
            if (isNameExist)
            {
                _logger.LogInformation("{FunctionName} Project namespace already exists. ProjectId: {ProjectId}," +
                                       " Namespace: {Namespace}", functionName, request.ProjectId, payload.Name);
                
                response.ErrorMessage = "Project namespace already exists";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }

            var entity = new ProjectNamespace
            {
                Id = Guid.CreateVersion7(),
                ProjectId = request.ProjectId,
                Name = payload.Name,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.Namespace.Add(entity);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new CreateProjectNamespaceData
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                Name = entity.Name,
                CreatedAt = entity.CreatedAt
            };

            _logger.LogInformation("{FunctionName} Project namespace created successfully. ProjectId: {ProjectId}," +
                                   " NamespaceId: {NamespaceId}", functionName, request.ProjectId, entity.Id);
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