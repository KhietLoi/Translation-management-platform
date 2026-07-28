using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeys;

public class GetTranslationKeysHandler : IRequestHandler<GetTranslationKeysQuery, GetTranslationKeysResponse>
{
    private readonly ILogger<GetTranslationKeysHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationKeysHandler
    (
        ILogger<GetTranslationKeysHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationKeysQuery, GetTranslationKeysResponse>

    public async Task<GetTranslationKeysResponse> Handle(GetTranslationKeysQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationKeysHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationKeysResponse();

        try
        {

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
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