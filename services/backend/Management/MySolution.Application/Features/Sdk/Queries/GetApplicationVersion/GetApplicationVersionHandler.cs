using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationVersion;

public class GetApplicationVersionHandler
    : IRequestHandler<GetApplicationVersionQuery, GetApplicationVersionResponse>
{
    private readonly ILogger<GetApplicationVersionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationAccessService _applicationAccessService;

    public GetApplicationVersionHandler(
        ILogger<GetApplicationVersionHandler> logger,
        IUnitOfWork unitOfWork,
        IApplicationAccessService applicationAccessService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _applicationAccessService = applicationAccessService;
    }

    public async Task<GetApplicationVersionResponse> Handle(
        GetApplicationVersionQuery request,
        CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationVersionHandler)} =>";
        _logger.LogInformation(functionName);

        var response = new GetApplicationVersionResponse();

        try
        {
            // 1. Get authorized application
            var application = await _applicationAccessService.GetApplicationAsync(cancellationToken);
            if (application == null)
            {
                response.ErrorMessage = "Application is not authorized.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }

            var projectId = application.ProjectId;

            // 2. Get active release
            var release = await _unitOfWork.TranslationRelease
                .GetAll()
                .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.IsActive, cancellationToken);
            if (release == null)
            {
                response.ErrorMessage = "This project does not have an active translation release.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            // 3. Build response
            response.Data = new GetApplicationVersionResponseData
            {
                ProjectId = projectId,
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
}