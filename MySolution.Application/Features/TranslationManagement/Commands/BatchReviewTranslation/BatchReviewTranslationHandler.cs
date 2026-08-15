using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;


namespace MySolution.Application.Features.TranslationManagement.Commands.BatchReviewTranslation;

public class BatchReviewTranslationHandler : IRequestHandler<BatchReviewTranslationCommand, BatchReviewTranslationResponse>
{
    private readonly ILogger<BatchReviewTranslationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public BatchReviewTranslationHandler
    (
        ILogger<BatchReviewTranslationHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
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
            
            var translations = await _unitOfWork.TranslationValue
                    .GetForBatchReviewAsync(translationIds, payload.ProjectId, payload.LanguageId, payload.NamespaceId);
            if (translations.Count != translationIds.Count)
            {
                _logger.LogWarning("{FunctionName} One or more translations were not found.", functionName);
                response.ErrorMessage = "One or more translations were not found.";
                response.WithStatus(HttpStatusCode.BadRequest);
                
                return response;
            }
            
            if (translations.Any(x => x.Status != TranslationStatus.Translated))
            {
                _logger.LogWarning("{FunctionName} One or more translations are not available for review.", functionName);
                response.ErrorMessage = "One or more translations are not available for review.";
                response.WithStatus(HttpStatusCode.BadRequest);
                
                return response;
            }

            var translationMap = translations.ToDictionary(x => x.Id);
            
            var approved = 0;
            var rejected = 0;
            foreach (var item in payload.Items)
            {
                if (!translationMap.TryGetValue(
                        item.TranslationValueId,
                        out var translation))
                {
                    _logger.LogWarning(
                        "{FunctionName} Translation {TranslationValueId} not found.",
                        functionName,
                        item.TranslationValueId);

                    response.ErrorMessage =
                        "One or more translations were not found.";

                    response.WithStatus(HttpStatusCode.BadRequest);

                    return response;
                }

                if (item.Status != TranslationStatus.Reviewed &&
                    item.Status != TranslationStatus.Rejected)
                {
                    _logger.LogWarning(
                        "{FunctionName} Invalid review status.",
                        functionName);

                    response.ErrorMessage =
                        "Invalid review status.";

                    response.WithStatus(HttpStatusCode.BadRequest);

                    return response;
                }

                if (item.Status == TranslationStatus.Rejected &&
                    string.IsNullOrWhiteSpace(item.RejectReason))
                {
                    _logger.LogWarning(
                        "{FunctionName} Reject reason is required.",
                        functionName);

                    response.ErrorMessage =
                        "Reject reason is required.";

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

                    translation.RejectionReason =
                        item.RejectReason?.Trim();

                    rejected++;
                }

                translation.UpdatedAt = DateTime.UtcNow;
            }
            
            await _unitOfWork.SaveAsync(cancellationToken);

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