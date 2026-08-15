using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;


namespace MySolution.Application.Features.TranslationManagement.Queries.GetReviewTranslations;

public class GetReviewTranslationsHandler : IRequestHandler<GetReviewTranslationsQuery, GetReviewTranslationsResponse>
{
    private readonly ILogger<GetReviewTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetReviewTranslationsHandler
    (
        ILogger<GetReviewTranslationsHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetReviewTranslationsQuery, GetReviewTranslationsResponse>

    public async Task<GetReviewTranslationsResponse> Handle(GetReviewTranslationsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetReviewTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetReviewTranslationsResponse();

        try
        {

            var translarions = await _unitOfWork.TranslationValue
                .GetReviewTranslationsAsync(
                    request.Payload.ProjectId,
                    request.Payload.LanguageId,
                    request.Payload.NamespaceId,
                    cancellationToken);
            
            _logger.LogInformation(
                "Found {Count} translations for review",
                translarions.Count);
            response.Data = new GetReviewTranslationsData
            {
                Items = translarions
                    .Select(x => new TranslationItem
                    {
                        TranslationValueId = x.Id,
                        TranslationKeyId = x.TranslationKeyId,
                        Key = x.TranslationKey.Key,
                        Value = x.Value,
                        Status = x.Status.ToString()
                    })
                    .ToList()
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