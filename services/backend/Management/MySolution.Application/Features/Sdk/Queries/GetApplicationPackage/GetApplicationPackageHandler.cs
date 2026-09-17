using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;

using Shared.Extensions;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationPackage;

public class GetApplicationPackageHandler
    : IRequestHandler<GetApplicationPackageQuery, GetApplicationPackageResponse>
{
    private readonly ILogger<GetApplicationPackageHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationAccessService _applicationAccessService;
    private readonly IAzureBlobService _azureBlobService;

    public GetApplicationPackageHandler(
        ILogger<GetApplicationPackageHandler> logger,
        IUnitOfWork unitOfWork,
        IApplicationAccessService applicationAccessService,
        IAzureBlobService azureBlobService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _applicationAccessService = applicationAccessService;
        _azureBlobService = azureBlobService;
    }

    public async Task<GetApplicationPackageResponse> Handle(
        GetApplicationPackageQuery request,
        CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationPackageHandler)} =>";
        _logger.LogInformation(functionName);

        var response = new GetApplicationPackageResponse();

        try
        {
            // 1. Get authorized application
            var application = await _applicationAccessService
                .GetApplicationAsync(cancellationToken);

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

            // 3. Check package exists
            var exists = await _azureBlobService.FileExistsAsync(
                release.BlobFileName,
                cancellationToken);

            if (!exists)
            {
                response.ErrorMessage = "Translation package was not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            // 4. Generate SAS URL
            var downloadUrl =
                await _azureBlobService.GenerateReadSasUrlAsync(
                    release.BlobFileName,
                    TimeSpan.FromMinutes(60),
                    cancellationToken);

            // 5. Build response
            response.Data = new GetApplicationPackageData
            {
                ProjectId = projectId,
                Version = release.Version,
                FileName = Path.GetFileName(release.BlobFileName),
                DownloadUrl = downloadUrl,
                PublishedAt = release.PublishedAt
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FunctionName} Unexpected error.",
                functionName);

            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }
}