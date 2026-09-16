using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Application.Queries.GetApplicationById;

public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, GetApplicationByIdResponse>
{
    private readonly ILogger<GetApplicationByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetApplicationByIdHandler
    (
        ILogger<GetApplicationByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetApplicationByIdQuery, GetApplicationByIdResponse>

    public async Task<GetApplicationByIdResponse> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetApplicationByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetApplicationByIdResponse();

        try
        {
            var application = await _unitOfWork.Application
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (application == null)
            {
                _logger.LogInformation(functionName + "Application not found.");
                
                response.ErrorMessage = "Application not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetApplicationByIdData
            {
                Id = application.Id,
                Description = application.Description,
                CreatedAt = application.CreatedAt,
                CreatedBy = application.CreatedBy,
                UpdatedAt = application.UpdatedAt,
                UpdatedBy = application.UpdatedBy,
                IsActive = application.IsActive,
                Name = application.Name
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