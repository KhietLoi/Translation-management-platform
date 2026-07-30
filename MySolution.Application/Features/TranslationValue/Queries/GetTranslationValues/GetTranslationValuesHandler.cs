using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationValue.Queries.GetTranslationValues;

public class GetTranslationValuesHandler : IRequestHandler<GetTranslationValuesQuery, GetTranslationValuesResponse>
{
    private readonly ILogger<GetTranslationValuesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationValuesHandler
    (
        ILogger<GetTranslationValuesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationValuesQuery, GetTranslationValuesResponse>

    public async Task<GetTranslationValuesResponse> Handle(GetTranslationValuesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationValuesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationValuesResponse();

        try
        {
            
            var entities = await _unitOfWork.TranslationValue.GetAsync(
                    request.TranslationKeyId,
                    request.NamespaceId,
                    request.LanguageId,
                    request.Status);
            
            response.Data = new GetTranslationValuesResult
            {
                translationValues = entities.Select(x => new GetTranslationValuesData
                {
                    Id = x.Id,
                    TranslationKeyId = x.TranslationKeyId,
                    TranslationKey = x.TranslationKey.Key,
                    NamespaceId = x.TranslationKey.NamespaceId,
                    NamespaceName = x.TranslationKey.Namespace.Name,
                    LanguageId = x.LanguageId,
                    LanguageCode = x.Language.Code,
                    Value = x.Value,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
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