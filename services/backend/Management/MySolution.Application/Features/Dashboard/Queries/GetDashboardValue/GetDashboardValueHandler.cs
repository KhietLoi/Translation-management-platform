using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;

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
            var projectIds = await _unitOfWork.Project
                .GetAll()
                .AsNoTracking()
                .Where(x => x.ProjectMembers.Any(pm => pm.UserId == _currentUser.UserId))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
            if (projectIds.Count == 0)
            {
                response.ErrorMessage = "No projects found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Calculate summary: total projects, total translation key, total translation values, translated value, pending review.
            var totalTranslationKeys = 0;
            if (projectIds.Count > 0)
            {
                totalTranslationKeys = await _unitOfWork.TranslationKey
                    .GetAll()
                    .CountAsync(x => projectIds.Contains(x.ProjectId), cancellationToken);
            }
            
            var translationStats = projectIds.Count == 0
                ? new TranslationDashboardStats()
                : await _unitOfWork.TranslationValue
                    .GetAll()
                    .AsNoTracking()
                    .Where(x => projectIds.Contains(x.TranslationKey.ProjectId))
                    .GroupBy(_ => 1)
                    .Select(g => new TranslationDashboardStats
                    {
                        Total = g.Count(),
                        Translated = g.Count(x =>
                            x.Status == TranslationStatus.Translated ||
                            x.Status == TranslationStatus.Reviewed ||
                            x.Status == TranslationStatus.Published),
                        PendingReview = g.Count(x => x.Status == TranslationStatus.Translated)
                    }).FirstOrDefaultAsync(cancellationToken) ?? new TranslationDashboardStats();
            
            var translationProgress = translationStats.Total == 0 ? 0 : Math.Round(translationStats.Translated * 100m / translationStats.Total,2);
            // Get language progress
            var languageResults = projectIds.Count == 0
                ? [] :await _unitOfWork.TranslationValue
                    .GetAll()
                    .Where(x => projectIds.Contains(x.TranslationKey.ProjectId))
                    .GroupBy(x => new
                    {
                        x.LanguageId,
                        x.Language.Code,
                        x.Language.Name
                    })
                    .Select(g => new DashboardLanguageProgressDto
                    {
                        LanguageId = g.Key.LanguageId,
                        LanguageCode = g.Key.Code,
                        LanguageName = g.Key.Name,
                        Total = g.Count(),
                        Translated = g.Count(x =>
                            x.Status == TranslationStatus.Translated ||
                            x.Status == TranslationStatus.Reviewed ||
                            x.Status == TranslationStatus.Published)
                    })
                    .OrderBy(x => x.LanguageCode)
                    .ToListAsync(cancellationToken);

            var languageProgress = languageResults
                .Select(x => new DashboardLanguageProgressDto()
                {
                    LanguageId = x.LanguageId,
                    LanguageCode = x.LanguageCode,
                    Progress = x.Total == 0 ? 0 : Math.Round(x.Translated * 100m / x.Total, 2)
                }).ToList();
            
            var notifications = await _unitOfWork.Notification
                .GetAll()
                .AsNoTracking()
                .Where(x => x.UserId == _currentUser.UserId)
                .Include(x => x.TriggeredByUser)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync(cancellationToken);
            
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