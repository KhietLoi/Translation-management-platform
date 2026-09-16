using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;


namespace MySolution.Application.Features.TranslationManagement.Commands.BatchReviewTranslation;

public class BatchReviewTranslationHandler : IRequestHandler<BatchReviewTranslationCommand, BatchReviewTranslationResponse>
{
    private readonly ILogger<BatchReviewTranslationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly INotificationService _notificationService;

    public BatchReviewTranslationHandler
    (
        ILogger<BatchReviewTranslationHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        INotificationService notificationService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    #region Implementation of IRequestHandler<in BatchReviewTranslationCommand, BatchReviewTranslationResponse>

    public async Task<BatchReviewTranslationResponse> Handle(BatchReviewTranslationCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(BatchReviewTranslationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new BatchReviewTranslationResponse();

        try
        {
            var translationIds = payload.Items
                .Select(x => x.TranslationValueId)
                .Distinct()
                .ToList();
            
            var translationValues = await _unitOfWork.TranslationValue
                .GetAll()
                .Include(x => x.TranslationKey)
                    .ThenInclude(x => x.Project)
                .Include(x => x.TranslationKey)
                    .ThenInclude(x => x.Namespace)
                .Where(x =>
                    translationIds.Contains(x.Id) &&
                    x.LanguageId == payload.LanguageId &&
                    x.TranslationKey.ProjectId == payload.ProjectId &&
                    x.TranslationKey.NamespaceId == payload.NamespaceId
                )
                .ToListAsync(cancellationToken);

            if (!translationValues.Any())
            {
                _logger.LogWarning("{FunctionName} One or more translations were not found.", functionName);
                
                response.ErrorMessage = "One or more translations were not found.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            if (translationValues.Any(x => x.Status != TranslationStatus.Translated))
            {
                _logger.LogWarning("{FunctionName} One or more translations are not available for review.", functionName);
                
                response.ErrorMessage = "One or more translations are not available for review.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var translationMap = translationValues.ToDictionary(x => x.Id);
            
            var approved = 0;
            var rejected = 0;
            foreach (var item in payload.Items)
            {
                if (!translationMap.TryGetValue(item.TranslationValueId, out var translation))
                {
                    _logger.LogWarning("{FunctionName} Translation {TranslationValueId} not found.", functionName, item.TranslationValueId);
                    
                    response.ErrorMessage = "One or more translations were not found.";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                if (item.Status != TranslationStatus.Reviewed && item.Status != TranslationStatus.Rejected)
                {
                    _logger.LogWarning("{FunctionName} Invalid review status.", functionName);

                    response.ErrorMessage = "Invalid review status.";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                if (item.Status == TranslationStatus.Rejected && string.IsNullOrWhiteSpace(item.RejectReason))
                {
                    _logger.LogWarning("{FunctionName} Reject reason is required.", functionName);
                    
                    response.ErrorMessage = "Reject reason is required.";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                if (item.Status == TranslationStatus.Reviewed)
                {
                    translation.Status = TranslationStatus.Reviewed;
                    approved++;
                }
                else
                {
                    translation.Status = TranslationStatus.Rejected;
                    translation.RejectionReason = item.RejectReason?.Trim();
                    rejected++;
                }

                translation.ReviewedAt = DateTime.UtcNow;
                translation.ReviewedBy = _currentUser.UserId;
            }
              
            await _unitOfWork.SaveAsync(cancellationToken);
            
            var sample = translationValues.First();
            var projectName = sample.TranslationKey.Project.Name;
            var namespaceName = sample.TranslationKey.Namespace.Name;

            await _notificationService.NotifyProjectAsync(
                payload.ProjectId,
                _currentUser.UserId,
                "Batch Review Completed",
                $"Reviewed {approved + rejected} translation(s) in project '{projectName}', " +
                $"namespace '{namespaceName}' " +
                $"(Approved: {approved}, Rejected: {rejected}).",
                NotificationType.Infor,
                $"/projects/{payload.ProjectId}/translations" +
                $"?languageId={payload.LanguageId}" +
                $"&namespaceId={payload.NamespaceId}",
                NotificationReferenceType.TranslationValue,
                payload.ProjectId,
                cancellationToken);

            response.Data = new BatchReviewTranslationData
            {
                Total = approved + rejected,
                Approved = approved,
                Rejected = rejected
            };
            
            response
                 .WithSuccess(true)
                 .WithStatus(HttpStatusCode.Created);
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