using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;


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
        var payload = request.Payload;
        var functionName = $"{nameof(GetReviewTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetReviewTranslationsResponse();

        try
        {
            var translations = await _unitOfWork.TranslationValue
                .GetAll()
                .AsNoTracking()
                .Include(x => x.TranslationKey)
                .Where(x =>
                    x.TranslationKey.ProjectId == payload.ProjectId &&
                    x.TranslationKey.NamespaceId == payload.NamespaceId &&
                    x.LanguageId == payload.LanguageId &&
                    x.Status == TranslationStatus.Translated
                )
                .OrderBy(x => x.TranslationKey.Key)
                .ToListAsync(cancellationToken);

            if (!translations.Any())
            {
                _logger.LogInformation(functionName + " No translations found.");
                
                response.ErrorMessage = "No translations found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetReviewTranslationsData
            {
                Items = translations
                    .Select(x => new TranslationItem
                    {
                        TranslationValueId = x.Id,
                        TranslationKeyId = x.TranslationKeyId,
                        Key = x.TranslationKey.Key,
                        Value = x.Value,
                        Status = x.Status.ToString()
                    }).ToList()
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