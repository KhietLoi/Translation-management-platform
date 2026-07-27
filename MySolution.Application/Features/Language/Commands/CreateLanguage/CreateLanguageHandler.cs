using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Language.Commands.CreateLanguage;

public class CreateLanguageHandler : IRequestHandler<CreateLanguageCommand, CreateLanguageResponse>
{
    private readonly ILogger<CreateLanguageHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public CreateLanguageHandler
    (
        ILogger<CreateLanguageHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in CreateLanguageCommand, CreateLanguageResponse>

    public async Task<CreateLanguageResponse> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(CreateLanguageHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateLanguageResponse();

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