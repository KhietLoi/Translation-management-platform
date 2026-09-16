using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;


namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValuesForBatch;

public class GetTranslationValuesForBatchHandler : IRequestHandler<GetTranslationValuesForBatchQuery, GetTranslationValuesForBatchResponse>
{
    private readonly ILogger<GetTranslationValuesForBatchHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationValuesForBatchHandler
    (
        ILogger<GetTranslationValuesForBatchHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationValuesForBatchQuery, GetTranslationValuesForBatchResponse>

    public async Task<GetTranslationValuesForBatchResponse> Handle(GetTranslationValuesForBatchQuery request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(GetTranslationValuesForBatchHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationValuesForBatchResponse();

        try
        {
            var translationValues = await _unitOfWork.TranslationValue
                .GetAll()
                .Include(x => x.TranslationKey)
                .Where(x =>
                    x.TranslationKey.ProjectId == payload.ProjectId &&
                    x.TranslationKey.NamespaceId == payload.NamespaceId &&
                    x.LanguageId == payload.LanguageId &&
                    (x.Status == TranslationStatus.Draft ||
                     x.Status == TranslationStatus.Missing ||
                     x.Status == TranslationStatus.Rejected))
                .OrderBy(x => x.TranslationKey.Key)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetTranslationValuesForBatchData
            {
                Items = translationValues
                    .Select(x => new TranslationValueBatchItem
                    {
                        TranslationValueId = x.Id,
                        TranslationKeyId = x.TranslationKeyId,
                        Key = x.TranslationKey.Key,
                        Value = x.Value,
                        Status = x.Status
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