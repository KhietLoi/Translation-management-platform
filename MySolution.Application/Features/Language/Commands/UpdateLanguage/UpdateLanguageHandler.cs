using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Language.Commands.UpdateLanguage;

public class UpdateLanguageHandler : IRequestHandler<UpdateLanguageCommand, UpdateLanguageResponse>
{
    private readonly ILogger<UpdateLanguageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateLanguageHandler
    (
        ILogger<UpdateLanguageHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateLanguageCommand, UpdateLanguageResponse>

    public async Task<UpdateLanguageResponse> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(UpdateLanguageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateLanguageResponse();

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