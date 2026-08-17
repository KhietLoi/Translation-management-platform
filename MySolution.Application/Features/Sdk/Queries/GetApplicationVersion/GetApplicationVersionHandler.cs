using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Sdk.Queries.GetApplicationVersion;

public class GetApplicationVersionHandler : IRequestHandler<GetApplicationVersionQuery, GetApplicationVersionResponse>
{
    private readonly ILogger<GetApplicationVersionHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationAccessService _applicationAccessService;

    public GetApplicationVersionHandler
    (
        ILogger<GetApplicationVersionHandler> logger,
		IUnitOfWork unitOfWork,
        IApplicationAccessService applicationAccessService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _applicationAccessService = applicationAccessService;
    }

    #region Implementation of IRequestHandler<in GetApplicationVersionQuery, GetApplicationVersionResponse>

    public async Task<GetApplicationVersionResponse> Handle(GetApplicationVersionQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationVersionHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApplicationVersionResponse();

        try
        {
            // 1. Check Application Access
            var canAccess = await _applicationAccessService.CanAccessProjectAsync(request.ProjectId, cancellationToken);
            if (!canAccess)
            {
                response.ErrorMessage = "Application is not authorized to access this project.";
                response.WithStatus(HttpStatusCode.Forbidden);
                return response;
            }

            // 2. Get Active Release
            var release = await _unitOfWork.TranslationRelease.GetActiveReleaseAsync(request.ProjectId, cancellationToken);
            if (release == null)
            {
                response.ErrorMessage = "This project does not have an active translation release.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            // 3. Build response
            response.Data =
                new GetApplicationVersionResponseData
                {
                    ProjectId = request.ProjectId,
                    Version = release.Version,
                    PublishedAt = release.PublishedAt
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