using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.ImportExport.Commands.ImportTranslations;

public class ImportTranslationsHandler : IRequestHandler<ImportTranslationsCommand, ImportTranslationsResponse>
{
    private readonly ILogger<ImportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public ImportTranslationsHandler
    (
        ILogger<ImportTranslationsHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in ImportTranslationsCommand, ImportTranslationsResponse>

    public async Task<ImportTranslationsResponse> Handle(ImportTranslationsCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ImportTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ImportTranslationsResponse();

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