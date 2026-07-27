using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Language.Commands.DeleteLanguage;

public class DeleteLanguageHandler : IRequestHandler<DeleteLanguageCommand, DeleteLanguageResponse>
{
    private readonly ILogger<DeleteLanguageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public DeleteLanguageHandler
    (
        ILogger<DeleteLanguageHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in DeleteLanguageCommand, DeleteLanguageResponse>

    public async Task<DeleteLanguageResponse> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteLanguageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new DeleteLanguageResponse();

        try
        {

            response.Ok();
        }
        catch (Exception exception)
        {
            exception.LogError(_logger, functionName);
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}