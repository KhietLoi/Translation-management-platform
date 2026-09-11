using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.Notification.Queries.GetNotificationDetail;

public class GetNotificationDetailHandler : IRequestHandler<GetNotificationDetailQuery, GetNotificationDetailResponse>
{
    private readonly ILogger<GetNotificationDetailHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetNotificationDetailHandler
    (
        ILogger<GetNotificationDetailHandler> logger,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<GetNotificationDetailResponse> Handle(
        GetNotificationDetailQuery request,
        CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetNotificationDetailHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetNotificationDetailResponse();

        try
        {
            var notification =
                await _unitOfWork.Notification.GetDetailAsync(
                    payload.NotificationId,
                    _currentUser.UserId,
                    cancellationToken);

            if (notification == null)
            {
                response.ErrorMessage = $"Notification with Id {payload.NotificationId} not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            object? detail = null;
            if (notification.ReferenceId.HasValue)
            {
                switch (notification.ReferenceType)
                {
                    case NotificationReferenceType.TranslationJob:
                        var job = await _unitOfWork.TranslationJob.GetByIdAsync(notification.ReferenceId.Value);
                        if (job != null)
                        {
                            detail = new
                            {
                                job.Id,
                                Type = job.Type.ToString(),
                                Status = job.Status.ToString(),
                                job.FileName,
                                job.DownloadUrl,
                                job.TotalRecords,
                                job.SuccessRecords,
                                job.FailedRecords,
                                job.SkippedRecords,
                                job.ErrorMessage,
                                job.CreatedAt,
                                job.StartedAt,
                                job.CompletedAt
                            };
                        }
                        break;

                    case NotificationReferenceType.TranslationRelease:

                        var release =
                            await _unitOfWork.TranslationRelease
                                .GetByIdAsync(notification.ReferenceId.Value, cancellationToken);
                        if (release != null)
                        {
                            detail = new
                            {
                                release.Id,
                                release.Version,
                                release.TotalKey,
                                release.DownloadUrl,
                                release.PublishedAt,
                                release.Notes
                            };
                        }

                        break;

                    case NotificationReferenceType.TranslationValue:

                        var translationValue =
                            await _unitOfWork.TranslationValue.GetByIdAsync(notification.ReferenceId.Value);

                        if (translationValue != null)
                        {
                            detail = new
                            {
                                translationValue.Id,
                                TranslationKey = translationValue.TranslationKey.Key,
                                Language = translationValue.Language.Name,
                                translationValue.Value,
                                Status =  translationValue.Status.ToString(),
                                translationValue.TranslatedAt,
                                translationValue.ReviewedAt,
                                translationValue.PublishedAt,
                                TranslatedBy = translationValue.Translator?.Username,
                                ReviewedBy = translationValue.Reviewer?.Username,
                                PublishedBy = translationValue.Publisher?.Username
                            };
                        }

                        break;
                }
            }

            response.Data = new GetNotificationDetailData
                {
                    NotificationId = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    IsRead = notification.IsRead,
                    NavigationUrl = notification.NavigationUrl,
                    TriggeredByUserId = notification.TriggeredByUserId,
                    TriggeredByUserName = notification.TriggeredByUser?.Username ?? string.Empty,
                    CreatedAt = notification.CreatedAt,
                    Detail = detail
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