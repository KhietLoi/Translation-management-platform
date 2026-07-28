using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationKey.Queries.SearchTranslationKeys;

public class SearchTranslationKeysHandler : IRequestHandler<SearchTranslationKeysQuery, SearchTranslationKeysResponse>
{
    private readonly ILogger<SearchTranslationKeysHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public SearchTranslationKeysHandler
    (
        ILogger<SearchTranslationKeysHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in SearchTranslationKeysQuery, SearchTranslationKeysResponse>

    public async Task<SearchTranslationKeysResponse> Handle(SearchTranslationKeysQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(SearchTranslationKeysHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new SearchTranslationKeysResponse();

        try
        {

            response.Ok();
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