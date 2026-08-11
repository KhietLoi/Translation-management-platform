using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseHistory;

public class GetReleaseHistoryHandler : IRequestHandler<GetReleaseHistoryQuery, GetReleaseHistoryResponse>
{
    private readonly ILogger<GetReleaseHistoryHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetReleaseHistoryHandler
    (
        ILogger<GetReleaseHistoryHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetReleaseHistoryQuery, GetReleaseHistoryResponse>

    public async Task<GetReleaseHistoryResponse> Handle(GetReleaseHistoryQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetReleaseHistoryHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetReleaseHistoryResponse();

        try
        {
            //Check projectId:
            var isProjectValid = await _unitOfWork.Project.ExistsAsync(request.ProjectId);
            if (!isProjectValid)
            {
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Load release:
            var releases = await _unitOfWork.TranslationRelease
                .GetReleaseHistoryAsync(
                    request.ProjectId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);
            
            response.Data = new GetReleaseHistoryData
            {
                Items = releases.Items
                    .Select(x => new GetReleaseHistoryItem
                    {
                        ReleaseId = x.Id,
                        ProjectId = x.ProjectId,
                        VersionNumber = x.Version,
                        PublishingUserId = x.PublishedBy,
                        ReleaseDate = x.PublishedAt,
                        Notes = x.Notes
                    })
                    .ToList(),

                TotalCount = releases.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
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