using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Language.Queries.GetLanguages;

public class GetLanguagesHandler : IRequestHandler<GetLanguagesQuery, GetLanguagesResponse>
{
    private readonly ILogger<GetLanguagesHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetLanguagesHandler
    (
        ILogger<GetLanguagesHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetLanguagesQuery, GetLanguagesResponse>

    public async Task<GetLanguagesResponse> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetLanguagesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetLanguagesResponse();

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