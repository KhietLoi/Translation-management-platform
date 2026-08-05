using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridHandler : IRequestHandler<GetApiKeyGridQuery, GetApiKeyGridResponse>
{
    private readonly ILogger<GetApiKeyGridHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetApiKeyGridHandler
    (
        ILogger<GetApiKeyGridHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetApiKeyGridQuery, GetApiKeyGridResponse>

    public async Task<GetApiKeyGridResponse> Handle(GetApiKeyGridQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApiKeyGridHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApiKeyGridResponse();

        try
        {

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