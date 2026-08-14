using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Notification.Queries.GetNotificationById;

public class GetNotificationByIdHandler : IRequestHandler<GetNotificationByIdQuery, GetNotificationByIdResponse>
{
    private readonly ILogger<GetNotificationByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetNotificationByIdHandler
    (
        ILogger<GetNotificationByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetNotificationByIdQuery, GetNotificationByIdResponse>

    public async Task<GetNotificationByIdResponse> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetNotificationByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetNotificationByIdResponse();

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