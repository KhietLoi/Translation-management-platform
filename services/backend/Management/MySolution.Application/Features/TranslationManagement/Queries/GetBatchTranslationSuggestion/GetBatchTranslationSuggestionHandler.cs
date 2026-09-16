using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.AI; 
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models.AI; 


namespace MySolution.Application.Features.TranslationManagement.Queries.GetBatchTranslationSuggestion;

public class GetBatchTranslationSuggestionHandler : IRequestHandler<GetBatchTranslationSuggestionQuery, GetBatchTranslationSuggestionResponse>
{
    private readonly ILogger<GetBatchTranslationSuggestionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITranslationSuggestionService _aiService; 
    private const string ReferenceLanguageCode = "vi-VN"; 

    public GetBatchTranslationSuggestionHandler
    (
        ILogger<GetBatchTranslationSuggestionHandler> logger,
        IUnitOfWork unitOfWork,
        ITranslationSuggestionService aiService 
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    #region Implementation of IRequestHandler<in GetBatchTranslationSuggestionQuery, GetBatchTranslationSuggestionResponse>

    public async Task<GetBatchTranslationSuggestionResponse> Handle(GetBatchTranslationSuggestionQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetBatchTranslationSuggestionHandler)} =>";
        var response = new GetBatchTranslationSuggestionResponse();
        var resultDataList = new List<GetBatchTranslationSuggestionData>();
        
        try
        {
            var targets = await _unitOfWork.TranslationValue
                .GetAll()
                .AsNoTrackingWithIdentityResolution()
                .Include(x => x.Language)
                .Include(x => x.TranslationKey)
                    .ThenInclude(k => k.TranslationValues)
                    .ThenInclude(v => v.Language)
                .Where(x => payload.TranslationValueIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            if (!targets.Any())
            {
                _logger.LogInformation("No translations found for {FunctionName}", functionName);
                
                response.ErrorMessage = "No valid targets found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var validTargets = new List<Domain.Entities.TranslationValue>();
            
            foreach (var target in targets)
            {
                if (!string.IsNullOrWhiteSpace(target.Value))
                {
                    continue;
                }

                if (target.Language.Code == ReferenceLanguageCode)
                {
                    continue;
                }

                var reference = target.TranslationKey.TranslationValues
                    .FirstOrDefault(x => x.Language.Code == ReferenceLanguageCode && !string.IsNullOrWhiteSpace(x.Value));

                if (reference == null)
                {
                    continue;
                }

                validTargets.Add(target);
            }
            
            var groupedTargets = validTargets.GroupBy(x => x.Language.Code);

            foreach (var group in groupedTargets)
            {
                var targetLanguage = group.Key;
                var dictToTranslate = new Dictionary<string, string>();
                
                foreach (var target in group)
                {
                    var referenceValue = target.TranslationKey.TranslationValues
                        .First(x => x.Language.Code == ReferenceLanguageCode).Value;
                    
                    dictToTranslate.Add(target.Id.ToString(), referenceValue);
                }

                if (!dictToTranslate.Any())
                {
                    continue;
                }
                
                var aiResponse = await _aiService.BatchSuggestAsync(new BatchTranslationSuggestionRequest
                {
                    SourceLanguage = ReferenceLanguageCode,
                    TargetLanguage = targetLanguage,
                    Data = dictToTranslate
                }, cancellationToken);
                
                foreach (var target in group)
                {
                    if (aiResponse.Suggestions.TryGetValue(target.Id.ToString(), out string? translatedText))
                    {
                        resultDataList.Add(new GetBatchTranslationSuggestionData
                        {
                            TranslationValueId = target.Id,
                            ReferenceLanguage = ReferenceLanguageCode,
                            TargetLanguage = targetLanguage,
                            Suggestion = translatedText
                        });
                    }
                }
            }
            
            response.Data = new GetBatchTranslationSuggestionResult
            {
                Data = resultDataList
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