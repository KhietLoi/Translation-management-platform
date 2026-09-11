using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
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
        var payload = request.Payload;
        var functionName = $"{nameof(GetApiKeyGridHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApiKeyGridResponse();

        try
        {
            var result = await _unitOfWork.ApiKey.GetGridAsync
            (
                payload.ProjectId,
                payload.ApplicationId,
                payload.Keyword,
                payload.IsRevoked,
                payload.Page,
                payload.Limit,
                cancellationToken
            );

            response.Data = new GetApiKeyGridResult
                {
                    Items = result.Items,
                    Paging = new PagingInfo
                    {
                        Page = payload.Page,
                        Limit = payload.Limit,
                        TotalItem = result.TotalItems,
                        TotalPage = (int)Math.Ceiling(result.TotalItems / (double)payload.Limit)
                    }
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