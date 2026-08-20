using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;

public class BatchUpdateTranslationHandler : IRequestHandler<BatchUpdateTranslationCommand, BatchUpdateTranslationResponse>
{
    private readonly ILogger<BatchUpdateTranslationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditLogService _auditLogService;

    public BatchUpdateTranslationHandler
    (
        ILogger<BatchUpdateTranslationHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IAuditLogService auditLogService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _auditLogService = auditLogService;
    }

    #region Implementation of IRequestHandler<in BatchUpdateTranslationCommand, BatchUpdateTranslationResponse>

    public async Task<BatchUpdateTranslationResponse> Handle(BatchUpdateTranslationCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(BatchUpdateTranslationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new BatchUpdateTranslationResponse();
        // Validate empty values
        var hasEmptyValue = payload.Items.Any(x => string.IsNullOrWhiteSpace(x.Value));
        if (hasEmptyValue)
        {
            response.ErrorMessage = "Translation value cannot be empty.";
            response.WithStatus(HttpStatusCode.BadRequest);
            return response;
        }
        
        try
        {
            // 1. Get distinct TranslationValue IDs
            var translationIds = payload.Items
                .Select(x => x.TranslationValueId)
                .Distinct()
                .ToList();

            // 2. Query all TranslationValues
            var translations =
                await _unitOfWork.TranslationValue
                    .GetForBatchTranslationAsync(
                        translationIds,
                        payload.ProjectId,
                        payload.LanguageId,
                        payload.NamespaceId);

            // 3. Validate that all submitted values
            //    are available for translation
            if (translations.Count != translationIds.Count)
            {
                response.ErrorMessage = "One or more translations were not found or are not available for translation.";
                response.WithStatus(HttpStatusCode.BadRequest);

                return response;
            }

            // 4. Create lookup dictionary
            var translationMap = translations.ToDictionary(x => x.Id);

            // 5. Store old values for audit
            var oldValues = new Dictionary<Guid, object>();

            var now = DateTime.UtcNow;

            // 6. Update all translations
            foreach (var item in payload.Items)
            {
                if (!translationMap.TryGetValue(item.TranslationValueId, out var translation))
                {

                    response.ErrorMessage = "One or more translations were not found.";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                // Save old value
                oldValues[translation.Id] = new
                {
                    translation.Value,
                    translation.Status
                };

                // Update value
                translation.Value = item.Value.Trim();
                translation.Status = payload.IsSubmit ? TranslationStatus.Translated : TranslationStatus.Draft;
                translation.UpdatedAt = now;
                translation.TranslatedBy = _currentUser.UserId;
                translation.TranslatedAt = now;

                translation.RejectionReason = null;
            }

            // 7. Audit all updated values
            foreach (var translation in translations)
            {
                var newValue = new
                {
                    translation.Value,
                    translation.Status
                };

                await _auditLogService.CreateAsync(
                    _currentUser.UserId,
                    payload.IsSubmit ? AuditAction.SubmitTranslation : AuditAction.Update,
                    AuditConstants.TranslationValue,
                    translation.Id,
                    request.Payload.ProjectId,
                    oldValues[translation.Id],
                    newValue);
            }

            // 8. Save once
            await _unitOfWork.SaveAsync(
                cancellationToken);

            // 9. Build response
            response.Data =
                new BatchUpdateTranslationData
                {
                    Total = translations.Count,

                    Items = translations
                        .Select(x =>
                            new BatchUpdatedTranslationValueItem
                            {
                                TranslationValueId = x.Id,
                                TranslationKeyId = x.TranslationKeyId,
                                LanguageId = x.LanguageId,
                                Value = x.Value,
                                Status = x.Status,
                                CreatedAt = x.CreatedAt,
                                UpdatedAt = x.UpdatedAt
                            })
                        .ToList()
                };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}
