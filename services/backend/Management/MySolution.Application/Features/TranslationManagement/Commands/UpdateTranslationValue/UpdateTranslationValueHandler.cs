using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.UpdateTranslationValue;

public class UpdateTranslationValueHandler : IRequestHandler<UpdateTranslationValueCommand, UpdateTranslationValueResponse>
{
    private readonly ILogger<UpdateTranslationValueHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUser _currentUserService;

    public UpdateTranslationValueHandler
    (
        ILogger<UpdateTranslationValueHandler> logger,
		IUnitOfWork unitOfWork,
        IAuditLogService auditLogService,
        ICurrentUser currentUserService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
    }

    #region Implementation of IRequestHandler<in UpdateTranslationValueCommand, UpdateTranslationValueResponse>

    public async Task<UpdateTranslationValueResponse> Handle(UpdateTranslationValueCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateTranslationValueHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateTranslationValueResponse();

        try
        {
            var entity = await _unitOfWork.TranslationValue
                .GetAll()
                .Include(x => x.TranslationKey)
                .Include(x => x.Language)
                .Include(x => x.Reviewer)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
            {
                _logger.LogInformation("{FunctionName} TranslationValue not found for Id: {Id}", functionName, request.Id);
                
                response.ErrorMessage = "TranslationValue not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            // Check if the translation value is already translated
            if (entity.Status is TranslationStatus.Translated)
            {
                _logger.LogInformation("{FunctionName} TranslationValue already exists for Id: {Id}", functionName, request.Id);
                
                response.ErrorMessage = "Cannot update a translation value that is already translated.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // Check if the translation value is under review
            if (entity.Status is TranslationStatus.Reviewed)
            {
                _logger.LogInformation("{FunctionName} TranslationValue is under review for Id: {Id}", functionName, request.Id);
                
                response.ErrorMessage = "Cannot update a translation value that is under review.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // Check if the translation value is already published
            if (entity.Status is TranslationStatus.Published)
            {
                _logger.LogInformation("{FunctionName} TranslationValue is published for Id: {Id}", functionName, request.Id);
                
                response.ErrorMessage = "Cannot update a translation value that is already published.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            
            // Save old value for audit logging
            var oldValue = new
            {
                entity.Value,
                entity.Status
            };
            
            var now = DateTime.UtcNow;
            entity.Value = payload.Value;
            entity.UpdatedAt = now;
            entity.Status  = TranslationStatus.Draft;
            entity.TranslatedBy = _currentUserService.UserId;
            entity.TranslatedAt = DateTime.UtcNow;
            
            var newValue = new
            {
                entity.Value,
                entity.Status
            };

            response.Data = new UpdateTranslationValueData
            {
                Id = entity.Id,
                TranslationKeyId = entity.TranslationKeyId,
                LanguageId = entity.LanguageId,
                Value = entity.Value,
                Status = TranslationStatus.Draft,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = now
            };
            
            // Audit log
            await _auditLogService.CreateAsync(
                _currentUserService.UserId,
                AuditAction.Update,
                AuditConstants.TranslationValue,
                entity.Id,
                entity.TranslationKey.ProjectId,
                oldValue,
                newValue);
            
            
            await _unitOfWork.SaveAsync(cancellationToken);

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