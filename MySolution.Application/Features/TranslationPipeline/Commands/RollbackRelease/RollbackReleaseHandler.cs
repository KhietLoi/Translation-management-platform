using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
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