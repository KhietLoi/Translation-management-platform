using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationManagement.Commands.ReviewTranslation;

public class ReviewTranslationHandler : IRequestHandler<ReviewTranslationCommand, ReviewTranslationResponse>
{
    private readonly ILogger<ReviewTranslationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUser _currentUser;

    public ReviewTranslationHandler
    (
        ILogger<ReviewTranslationHandler> logger,
		IUnitOfWork unitOfWork,
        IAuditLogService auditLogService,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in ReviewTranslationCommand, ReviewTranslationResponse>

    public async Task<ReviewTranslationResponse> Handle(ReviewTranslationCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ReviewTranslationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ReviewTranslationResponse();

        try
        {
            var entity = await _unitOfWork.TranslationValue.GetByIdTrackingAsync(request.Id);
            if (entity == null)
            {
                response.ErrorMessage = $"Translation with Id {request.Id} not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (entity.Status != TranslationStatus.Translated)
            {
                response.ErrorMessage = $"Translation with Id {request.Id} is not translated.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var oldValue =  new
            {
                entity.Status
            };

            entity.Status = TranslationStatus.Reviewed;
            entity.ReviewedAt = DateTime.UtcNow;
            entity.ReviewedBy = _currentUser.UserId;

            var newValue = new
            {
                entity.Status
            };
            
            await _auditLogService.CreateAsync
            (
                _currentUser.UserId,
                AuditAction.ReviewTranslation,
                AuditConstants.TranslationValue,
                entity.Id,
                oldValue,
                newValue);

            response.Data = new ReviewTranslationData
            {
                Id = entity.Id,
                Status = entity.Status,
                ReviewerId = entity.ReviewedBy ?? Guid.Empty,
                ReviewedAt = entity.ReviewedAt ?? DateTime.MinValue,

            };
            
            await _unitOfWork.SaveAsync(cancellationToken);
            
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