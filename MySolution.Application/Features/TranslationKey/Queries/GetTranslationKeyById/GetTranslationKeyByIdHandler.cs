using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;

public class GetTranslationKeyByIdHandler : IRequestHandler<GetTranslationKeyByIdQuery, GetTranslationKeyByIdResponse>
{
    private readonly ILogger<GetTranslationKeyByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetTranslationKeyByIdHandler
    (
        ILogger<GetTranslationKeyByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetTranslationKeyByIdQuery, GetTranslationKeyByIdResponse>

    public async Task<GetTranslationKeyByIdResponse> Handle(GetTranslationKeyByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetTranslationKeyByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetTranslationKeyByIdResponse();

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