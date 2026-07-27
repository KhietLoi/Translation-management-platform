using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Project.Queries.GetProjectNamspaces;

public class GetProjectNamspacesHandler : IRequestHandler<GetProjectNamspacesQuery, GetProjectNamspacesResponse>
{
    private readonly ILogger<GetProjectNamspacesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetProjectNamspacesHandler
    (
        ILogger<GetProjectNamspacesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetProjectNamspacesQuery, GetProjectNamspacesResponse>

    public async Task<GetProjectNamspacesResponse> Handle(GetProjectNamspacesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetProjectNamspacesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProjectNamspacesResponse();

        try
        {
            var namespaces = await _unitOfWork.Namespace.GetByProjectIdAsync(request.ProjectId);

            response.Data = new GetProjectNamespacesResult
            {
                Namespaces = namespaces.Select(x => new GetProjectNamespaceData
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
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