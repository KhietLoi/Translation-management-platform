using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Sdk.Queries.GetApplicationTranslations;

public class GetApplicationTranslationsHandler : IRequestHandler<GetApplicationTranslationsQuery, GetApplicationTranslationsResponse>
{
    private readonly ILogger<GetApplicationTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationAccessService _applicationAccessService;
    private readonly IPublishedTranslationService _publishedTranslationService;

    public GetApplicationTranslationsHandler
    (
        ILogger<GetApplicationTranslationsHandler> logger,
		IUnitOfWork unitOfWork,
        IApplicationAccessService applicationAccessService,
        IPublishedTranslationService publishedTranslationService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _applicationAccessService = applicationAccessService;
        _publishedTranslationService = publishedTranslationService;
    }

    #region Implementation of IRequestHandler<in GetApplicationTranslationsQuery, GetApplicationTranslationsResponse>

    public async Task<GetApplicationTranslationsResponse> Handle(GetApplicationTranslationsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApplicationTranslationsResponse();

        try
        {
            /*var canAccess =  await _applicationAccessService.CanAccessProjectAsync(request.ProjectId, cancellationToken);
            if (!canAccess)
            {
                response.ErrorMessage = "This project is not authorized to access this application.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }*/
            
            
            var application = await _applicationAccessService
                .GetApplicationAsync(cancellationToken);

            if (application == null)
            {
                response.ErrorMessage = "Application is not authorized.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }

            var projectId = application.ProjectId;
            var release = await _unitOfWork.TranslationRelease.GetActiveReleaseAsync(projectId, cancellationToken);
            if (release == null)
            {
                response.ErrorMessage = "This project is not authorized to access this application.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }
            
            var translations =
                await _publishedTranslationService.GetTranslationsAsync(release.BlobFileName, request.Language, cancellationToken);

            response.Data = new GetApplicationTranslationsData
            {
                ProjectId = application.ProjectId,
                Language = request.Language,
                Version = release.Version,
                Translations = translations
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
            
            _logger.LogInformation(
                "{FunctionName} => Successfully retrieved {Count} translations for ProjectId: {ProjectId}, Language: {Language}, Version: {Version}",
                functionName,
                translations.Count,
                application.ProjectId,
                request.Language,
                release.Version);
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