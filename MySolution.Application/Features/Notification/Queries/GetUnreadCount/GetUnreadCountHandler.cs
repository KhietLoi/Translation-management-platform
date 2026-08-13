using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Notification.Queries.GetUnreadCount;

public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, GetUnreadCountResponse>
{
    private readonly ILogger<GetUnreadCountHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetUnreadCountHandler
    (
        ILogger<GetUnreadCountHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetUnreadCountQuery, GetUnreadCountResponse>

    public async Task<GetUnreadCountResponse> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetUnreadCountHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetUnreadCountResponse();

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