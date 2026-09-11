using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;

public class RollbackReleaseHandler : IRequestHandler<RollbackReleaseCommand, RollbackReleaseResponse>
{
    private readonly ILogger<RollbackReleaseHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public RollbackReleaseHandler
    (
        ILogger<RollbackReleaseHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in RollbackReleaseCommand, RollbackReleaseResponse>

    public async Task<RollbackReleaseResponse> Handle(RollbackReleaseCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(RollbackReleaseHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new RollbackReleaseResponse();

        try
        {
            var targetRelease = await _unitOfWork.TranslationRelease.GetByIdAsync(request.ReleaseId, cancellationToken);
            if (targetRelease is null)
            {
                response.ErrorMessage = "Release not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (targetRelease.IsActive)
            {
                response.ErrorMessage = "Release is already active.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

         
            var currentActiveRelease =
                await _unitOfWork.TranslationRelease.GetCurrentActiveAsync(targetRelease.ProjectId, cancellationToken);


            await _unitOfWork.OpenTransactionAsync(cancellationToken);

            try
            {  
                if (currentActiveRelease is not null)
                {
                    currentActiveRelease.IsActive = false;
                }

                targetRelease.IsActive = true;
                await _unitOfWork.SaveAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                response.Data = new RollbackReleaseData
                {
                    ReleaseId = targetRelease.Id,
                    ProjectId = targetRelease.ProjectId,
                    Version = targetRelease.Version,
                    IsActive = true
                };
                response
                    .WithSuccess(true)
                    .WithStatus(HttpStatusCode.OK);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
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