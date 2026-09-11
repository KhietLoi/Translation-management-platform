using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Project.Commands.DeleteProjectNamespace;

public class DeleteProjectNamespaceHandler : IRequestHandler<DeleteProjectNamespaceCommand, DeleteProjectNamespaceResponse>
{
    private readonly ILogger<DeleteProjectNamespaceHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteProjectNamespaceHandler
    (
        ILogger<DeleteProjectNamespaceHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteProjectNamespaceCommand, DeleteProjectNamespaceResponse>

    public async Task<DeleteProjectNamespaceResponse> Handle(DeleteProjectNamespaceCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteProjectNamespaceHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteProjectNamespaceResponse();

        try
        {
            var projectnamespace = await _unitOfWork.Namespace.GetByIdAsync(request.Id);
            if (projectnamespace == null)
            {
                response.ErrorMessage = "Namespace not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            _unitOfWork.Namespace.Delete(projectnamespace);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new DeleteProjectNamespaceData
            {
                Id = projectnamespace.Id,
                ProjectId = projectnamespace.ProjectId,
                Name = projectnamespace.Name,
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