using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.AI;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models.AI;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationSuggestion;

public class GetTranslationSuggestionHandler : IRequestHandler<GetTranslationSuggestionQuery, GetTranslationSuggestionResponse>
{
    private readonly ILogger<GetTranslationSuggestionHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private const string ReferenceLanguageCode = "vi-VN";
    private readonly ITranslationSuggestionService _aiService;

    public GetTranslationSuggestionHandler
    (
        ILogger<GetTranslationSuggestionHandler> logger,
		IUnitOfWork unitOfWork,
        ITranslationSuggestionService aiService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    #region Implementation of IRequestHandler<in GetTranslationSuggestionQuery, GetTranslationSuggestionResponse>

    public async Task<GetTranslationSuggestionResponse> Handle(GetTranslationSuggestionQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetTranslationSuggestionHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationSuggestionResponse();

        try
        {
            var target = await _unitOfWork.TranslationValue
                .GetAll()
                .AsNoTrackingWithIdentityResolution()
                .Include (x => x.Language)
                .Include (x => x.TranslationKey)
                    .ThenInclude (k => k.TranslationValues)
                    .ThenInclude (v => v.Language)
                .FirstOrDefaultAsync(x => x.Id == payload.TranslationValueId, cancellationToken);

            if (target is null)
            {
                response.ErrorMessage = "No target found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            if (target.Status != TranslationStatus.Draft &&
                target.Status != TranslationStatus.Rejected &&
                target.Status != TranslationStatus.Missing)
            {
                response.ErrorMessage = "AI suggestion is only available for draft or rejected or Missing translations.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            if (target.Language.Code == ReferenceLanguageCode)
            {
                response.ErrorMessage = "vi-VN is the reference language and cannot be the target";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var reference = target.TranslationKey.TranslationValues
                .FirstOrDefault(x => x.Language.Code == ReferenceLanguageCode && !string.IsNullOrWhiteSpace(x.Value));
            if (reference is null)
            {
                response.ErrorMessage = "Vietnamese reference translation is missing or empty.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var context = BuildContext(target);
            
            var aiResult = await _aiService.SuggestAsync(
                new TranslationSuggestionRequest
                {
                    SourceText = reference.Value,
                    SourceLanguage = ReferenceLanguageCode,
                    TargetLanguage = target.Language.Code,
                    Context = context
                }, cancellationToken);

            response.Data = new GetTranslationSuggestionData
            {
                TranslationValueId = target.Id,
                ReferenceLanguage = ReferenceLanguageCode,
                TargetLanguage = target.Language.Code,
                Suggestion = aiResult.Suggestion
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
    private static string BuildContext(Domain.Entities.TranslationValue target)
    {
        var key = target.TranslationKey.Key;
        var description = target.TranslationKey.Description;
        if (string.IsNullOrWhiteSpace(description)) return key;

        return $"{key}. {description}";
    }
}
