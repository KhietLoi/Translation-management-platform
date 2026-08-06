using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Application.Command.DeleteApplication;

public class DeleteApplicationHandler : IRequestHandler<DeleteApplicationCommand, DeleteApplicationResponse>
{
    private readonly ILogger<DeleteApplicationHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteApplicationHandler
    (
        ILogger<DeleteApplicationHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteApplicationCommand, DeleteApplicationResponse>

    public async Task<DeleteApplicationResponse> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteApplicationHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteApplicationResponse();

        try
        {
            //Check if application exists
            var application = await _unitOfWork.Application.GetByIdAsync(request.Id);
            if (application == null)
            {
                response.ErrorMessage = "Application not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Delete Application
             _unitOfWork.Application.Delete(application);
            await _unitOfWork.SaveAsync(cancellationToken);

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