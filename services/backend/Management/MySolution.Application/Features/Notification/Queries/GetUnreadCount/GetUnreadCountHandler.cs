using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Notification.Queries.GetUnreadCount;

public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, GetUnreadCountResponse>
{
    private readonly ILogger<GetUnreadCountHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetUnreadCountHandler
    (
        ILogger<GetUnreadCountHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in GetUnreadCountQuery, GetUnreadCountResponse>

    public async Task<GetUnreadCountResponse> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetUnreadCountHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetUnreadCountResponse();

        try
        {
            int count = await _unitOfWork.Notification
                .GetAll()
                .CountAsync(x => x.UserId == _currentUser.UserId && !x.IsRead, cancellationToken);
            
            response.Data = new GetUnreadCountData
            {
                Count = count
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