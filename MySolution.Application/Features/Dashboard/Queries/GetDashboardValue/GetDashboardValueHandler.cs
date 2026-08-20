using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Dashboard.Queries.GetDashboardValue;

public class GetDashboardValueHandler : IRequestHandler<GetDashboardValueQuery, GetDashboardValueResponse>
{
    private readonly ILogger<GetDashboardValueHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetDashboardValueHandler
    (
        ILogger<GetDashboardValueHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in GetDashboardValueQuery, GetDashboardValueResponse>

    public async Task<GetDashboardValueResponse> Handle(GetDashboardValueQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetDashboardValueHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetDashboardValueResponse();

        try
        {
            // Get user:
            var projectIds = await _unitOfWork.Project.GetAccessibleProjectIdsAsync(_currentUser.UserId,cancellationToken);
            if (projectIds.Count == 0)
            {
                response.ErrorMessage = "No projects found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Calculate summary: total projects, total translation key, total translation values, translated value, pending review.
            var totalTranslationKeys = await _unitOfWork.TranslationKey
                .CountByProjectIdsAsync(projectIds, cancellationToken);
            
            var translationStats = await _unitOfWork.TranslationValue
                .GetDashboardStatsAsync(projectIds, cancellationToken);
            
            var translationProgress =
                translationStats.Total == 0 ? 0 : Math.Round(translationStats.Translated * 100m / translationStats.Total,2);
            // Get language progress
            var languageResults =
                await _unitOfWork.TranslationValue
                    .GetLanguageProgressAsync(
                        projectIds,
                        cancellationToken);

            var languageProgress = languageResults
                .Select(x => new LanguageProgressDto
                {
                    LanguageId = x.LanguageId,
                    LanguageCode = x.LanguageCode,
                    Progress = x.Total == 0 ? 0 : Math.Round(x.Translated * 100m / x.Total, 2)
                }).ToList();
            
           // var auditLogs = await _unitOfWork.AuditLog.GetRecentActivitiesAsync(projectIds, 10, cancellationToken);
            /*
            var recentActivities = auditLogs
                .Select(x => new RecentActivityDto
                {
                    Id = x.Id,
                    ActorName = x.User.Username,
                    Action = x.Action,
                    EntityName = x.EntityName,
                    EntityId = x.EntityId,
                    ProjectName =x.Project?.Name,
                    CreatedAt = x.CreatedAt
                }).ToList();
                */
            var (notifications, _) = await _unitOfWork.Notification.GetAsync(
                _currentUser.UserId,
                projectId: null,
                isRead: null,
                pageNumber: 1,
                pageSize: 10,
                cancellationToken);

            var recentActivities = notifications
                .Select(x => new RecentActivityDto
                {
                    Id = x.Id,
                    ActorName = x.TriggeredByUser?.Username ?? "System",
                    Message = x.Message,
                    ProjectId = x.ProjectId,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt
                })
                .ToList();

            response.Data = new GetDashboardValueResult
            {
                DashboardSummary = new DashboardSummaryDto()
                {
                    TotalProjects = projectIds.Count,
                    TotalTranslationKeys = totalTranslationKeys,
                    TranslationProgress = translationProgress,
                    PendingReview = translationStats.PendingReview
                },
                LanguageProgress = languageProgress,
                RecentActivities = recentActivities
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