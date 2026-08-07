using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.ImportExport.Commands.ExportTranslations;

public class ExportTranslationsHandler : IRequestHandler<ExportTranslationsCommand, ExportTranslationsResponse>
{
    private readonly ILogger<ExportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IExportService _exportService;
    private readonly IMessageSender _messageSender;

    public ExportTranslationsHandler
    (
        ILogger<ExportTranslationsHandler> logger,
		IUnitOfWork unitOfWork,
        IExportService exportService,
        IMessageSender messageSender
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _exportService = exportService;
        _messageSender = messageSender;
    }

    #region Implementation of IRequestHandler<in ExportTranslationsCommand, ExportTranslationsResponse>

    public async Task<ExportTranslationsResponse> Handle(ExportTranslationsCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(ExportTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ExportTranslationsResponse();

        try
        {
            var job = new TranslationJob
            {
                Id = Guid.CreateVersion7(),
                ProjectId = payload.ProjectId,
                Type = TranslationJobType.Export,
                Status = TranslationJobStatus.Pending,
                ExportFormat = payload.Format,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.TranslationJob.Add(job);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            await _messageSender.SendMessage<ExportTranslationsEvent>(new ExportTranslationsEvent
            {
                JobId = job.Id,
            }, cancellationToken);
            
            //var result = await _exportService.ExportAsync(payload.ProjectId, payload.Format, cancellationToken);
            response.Data = new ExportTranslationData
            {
                JobId = job.Id
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