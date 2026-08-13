using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.TranslationPipeline.Commands.ExportTranslations;

public class ExportTranslationsHandler : IRequestHandler<ExportTranslationsCommand, ExportTranslationsResponse>
{
    private readonly ILogger<ExportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageSender _messageSender;
    private readonly ICurrentUser _currentUser;

    public ExportTranslationsHandler
    (
        ILogger<ExportTranslationsHandler> logger,
		IUnitOfWork unitOfWork,
        IMessageSender messageSender,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _messageSender = messageSender;
        _currentUser = currentUser;
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
            // Validate project:
            var isProjectValid = await _unitOfWork.Project.ExistsAsync(payload.ProjectId);
            if (!isProjectValid)
            {
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var job = new TranslationJob
            {
                Id = Guid.CreateVersion7(),
                ProjectId = payload.ProjectId,
                Type = TranslationJobType.Export,
                Status = TranslationJobStatus.Pending,
                FileType = payload.Format,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId
            };
            
            await _unitOfWork.TranslationJob.Add(job);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            await _messageSender.SendMessage<ExportTranslationsEvent>(new ExportTranslationsEvent
            {
                JobId = job.Id,
            }, cancellationToken);
            
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