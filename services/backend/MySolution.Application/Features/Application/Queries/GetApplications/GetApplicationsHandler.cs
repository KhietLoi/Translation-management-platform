using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Application.Queries.GetApplications;

public class GetApplicationsHandler : IRequestHandler<GetApplicationsQuery, GetApplicationsResponse>
{
    private readonly ILogger<GetApplicationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetApplicationsHandler
    (
        ILogger<GetApplicationsHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetApplicationsQuery, GetApplicationsResponse>

    public async Task<GetApplicationsResponse> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApplicationsResponse();

        try
        {
            
            var applications = await _unitOfWork.Application
                .GetAll()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            response.Data = new GetApplicationsResult
            {
                Applications = applications.Select(x => new GetApplicationData
                {
                    ApplicationId = x.Id,
                    ApplicationName = x.Name,
                    ProjectId = x.ProjectId
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