using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.TranslationPipeline.Application.Features.ExportTranslations;

public class ExportTranslationsHandler : IRequestHandler<ExportTranslationsCommand, ExportTranslationsResponse>
{
    private readonly ILogger<ExportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public ExportTranslationsHandler
    (
        ILogger<ExportTranslationsHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in ExportTranslationsCommand, ExportTranslationsResponse>

    public async Task<ExportTranslationsResponse> Handle(ExportTranslationsCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ExportTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ExportTranslationsResponse();

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