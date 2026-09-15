using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var isProjectValid = await _unitOfWork.Project
                .GetAll()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.ProjectId, cancellationToken);
            if (!isProjectValid)
            {
                _logger.LogError($"Project {request.ProjectId} does not exist.");
                
                response.ErrorMessage = "Project does not exist.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var query = from release in _unitOfWork.TranslationRelease
                    .GetAll()
                    .AsNoTracking()
                join user in _unitOfWork.User
                        .GetAll()
                        .AsNoTracking()
                    on release.PublishedBy equals user.Id
                where release.ProjectId == request.ProjectId
                select new GetReleaseHistoryItem
                {
                    ReleaseId = release.Id,
                    ProjectId = release.ProjectId,
                    BlobFileName = release.BlobFileName,
                    DownloadUrl = release.DownloadUrl,
                    VersionNumber = release.Version,
                    PublishingUserId = release.PublishedBy,
                    PublishingUserName = user.Username,
                    ReleaseDate = release.PublishedAt,
                    Notes = release.Notes,
                    TotalKey = release.TotalKey,
                    IsActive = release.IsActive
                };
            
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.VersionNumber)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetReleaseHistoryData
            {
                Items = items,
                TotalCount = totalCount,
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

