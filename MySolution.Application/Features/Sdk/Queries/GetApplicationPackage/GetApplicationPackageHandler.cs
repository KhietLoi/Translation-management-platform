using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Sdk.Queries.GetApplicationPackage;

public class GetApplicationPackageHandler : IRequestHandler<GetApplicationPackageQuery, GetApplicationPackageResponse>
{
    private readonly ILogger<GetApplicationPackageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationAccessService _applicationAccessService;
    private readonly IAzureBlobService _azureBlobService;

    public GetApplicationPackageHandler
    (
        ILogger<GetApplicationPackageHandler> logger,
		IUnitOfWork unitOfWork,
        IApplicationAccessService applicationAccessService,
        IAzureBlobService azureBlobService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _applicationAccessService = applicationAccessService;
        _azureBlobService = azureBlobService;
    }

    #region Implementation of IRequestHandler<in GetApplicationPackageQuery, GetApplicationPackageResponse>

    public async Task<GetApplicationPackageResponse> Handle(GetApplicationPackageQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationPackageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApplicationPackageResponse();

        try
        {
            var canAccess = await _applicationAccessService.CanAccessProjectAsync(request.ProjectId, cancellationToken);
            if (!canAccess)
            {
                response.ErrorMessage = "Application is not authorized to access this project.";
                response.WithStatus(HttpStatusCode.Forbidden);
                return response;
            }
            
            var release = await _unitOfWork.TranslationRelease.GetActiveReleaseAsync(request.ProjectId, cancellationToken);
            if (release == null)
            {
                response.ErrorMessage = "No active translation release was found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var exists = await _azureBlobService.FileExistsAsync(release.BlobFileName, cancellationToken);
            if (!exists)
            {
                response.ErrorMessage = "Translation package was not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            // Generate SAS URL
            var downloadUrl =
                await _azureBlobService.GenerateReadSasUrlAsync(release.BlobFileName, TimeSpan.FromMinutes(60), cancellationToken);

            // 5. Build response
            response.Data = new GetApplicationPackageData
                {
                    ProjectId = request.ProjectId,
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
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}