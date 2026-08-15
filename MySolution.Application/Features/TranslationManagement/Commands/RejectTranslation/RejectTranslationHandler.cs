using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Realtime;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationManagement.Commands.RejectTranslation;

public class RejectTranslationHandler : IRequestHandler<RejectTranslationCommand, RejectTranslationResponse>
{
    private readonly ILogger<RejectTranslationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUser _currentUser;
    private readonly INotificationService _notificationService;
    
    public RejectTranslationHandler
    (
        ILogger<RejectTranslationHandler> logger,
		IUnitOfWork unitOfWork,
        IAuditLogService auditLogService,
        ICurrentUser currentUser,
        INotificationService notificationService

    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    #region Implementation of IRequestHandler<in RejectTranslationCommand, RejectTranslationResponse>

    public async Task<RejectTranslationResponse> Handle(RejectTranslationCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(RejectTranslationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new RejectTranslationResponse();

        try
        {
            var entity = await _unitOfWork.TranslationValue.GetByIdTrackingAsync(request.Id);
            if (entity == null)
            {
                response.ErrorMessage = "Translation value not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (entity.Status != TranslationStatus.Translated)
            {
                response.ErrorMessage = "Only translated item can be rejected";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.Payload.Reason))
            {
                response.ErrorMessage = "Reject reason is required";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var oldValue = new
            {
                entity.Status
            };

            entity.Status = TranslationStatus.Rejected;
            entity.RejectionReason = request.Payload.Reason;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ReviewedBy = _currentUser.UserId;
            entity.ReviewedAt = DateTime.UtcNow;

            var newValue = new
            {
                entity.Status,
                entity.RejectionReason,
                entity.ReviewedBy,
                entity.ReviewedAt
            };

            await _auditLogService.CreateAsync(
                
                _currentUser.UserId,
                AuditAction.RejectTranslation,
                AuditConstants.TranslationValue,
                entity.Id,
                oldValue,
                newValue
            );

            await _unitOfWork.SaveAsync(cancellationToken);
            await _notificationService.NotifyProjectAsync(
                entity.TranslationKey.ProjectId,
                _currentUser.UserId,
                "Review Rejected",
                $"Rejected translation key '{entity.TranslationKey.Key}'.",
                NotificationType.Error,
                $"/projects/{entity.TranslationKey.ProjectId}/translations",
                NotificationReferenceType.TranslationValue,
                entity.Id,
                cancellationToken);

            response.Data = new RejectTranslationData
            {
                Id = entity.Id,
                Status = entity.Status,
                RejectionReason = entity.RejectionReason,
                UpdatedAt = entity.UpdatedAt ?? DateTime.MinValue
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception exception)
        {
            exception.LogError(_logger, functionName);
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}