using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectNamespace;

public class UpdateProjectNamespaceHandler : IRequestHandler<UpdateProjectNamespaceCommand, UpdateProjectNamespaceResponse>
{
    private readonly ILogger<UpdateProjectNamespaceHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectNamespaceHandler
    (
        ILogger<UpdateProjectNamespaceHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateProjectNamespaceCommand, UpdateProjectNamespaceResponse>

    public async Task<UpdateProjectNamespaceResponse> Handle(UpdateProjectNamespaceCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateProjectNamespaceHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateProjectNamespaceResponse();

        try
        {
            
            var projectnamespace = await _unitOfWork.Namespace.GetByIdAsync(request.Id);
            if (projectnamespace == null)
            {
                response.ErrorMessage = "Namespace not found";
                response.StatusCode  = HttpStatusCode.NotFound;
                return response;
            }
            
            //Check namespace name is already existing in project:
            var isExistingNamespace = await _unitOfWork.Namespace.ExistsAsync(payload.ProjectId, payload.Name, request.Id);
            if (isExistingNamespace)
            {
                response.ErrorMessage = "Namespace already exists";
                response.StatusCode  = HttpStatusCode.Conflict;
                return response;
            }

            //Update namespace:
            projectnamespace.Name = payload.Name;
            projectnamespace.ProjectId = payload.ProjectId;
            projectnamespace.UpdatedAt = DateTime.UtcNow;
            
            _logger.LogInformation(
                "Loitest: CreatedAt: {CreatedAt} - Kind: {Kind}",
                projectnamespace.CreatedAt,
                projectnamespace.CreatedAt.Kind);

            _logger.LogInformation(
                "Loitest: UpdatedAt: {UpdatedAt} - Kind: {Kind}",
                projectnamespace.UpdatedAt,
                projectnamespace.UpdatedAt?.Kind);
            
            _unitOfWork.Namespace.Update(projectnamespace);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new UpdateProjectNamespaceData
            {
                Id = projectnamespace.Id,
                Name = projectnamespace.Name,
                CreatedAt = projectnamespace.CreatedAt,
                ProjectId = projectnamespace.ProjectId,
                UpdatedAt = DateTime.UtcNow
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