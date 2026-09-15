using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;

public class PublishTranslationsHandler : IRequestHandler<PublishTranslationsCommand, PublishTranslationsResponse>
{
    private readonly ILogger<PublishTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageSender _messageSender;
    private readonly ICurrentUser _currentUser;

    public PublishTranslationsHandler
    (
        ILogger<PublishTranslationsHandler> logger,
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

    #region Implementation of IRequestHandler<in PublishTranslationsCommand, PublishTranslationsResponse>

    public async Task<PublishTranslationsResponse> Handle(PublishTranslationsCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(PublishTranslationsHandler)} =>";
        
        var response = new PublishTranslationsResponse();

        try
        {
            //Validate Project:
            var project = await _unitOfWork.Project
                .GetAll()
                .AsNoTracking()
                .AnyAsync(p => p.Id == request.Payload.ProjectId, cancellationToken);
            if (!project)
            {
                _logger.LogInformation("Project {ProjectId} does not exist", request.Payload.ProjectId);
                
                response.ErrorMessage = $"Project with id {payload.ProjectId} does not exist.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var job = new TranslationJob
            {
                Id = Guid.CreateVersion7(),
                ProjectId = request.Payload.ProjectId,
                Notes = request.Payload.Notes,
                Type = TranslationJobType.Publish,
                Status = TranslationJobStatus.Pending,
                CreatedBy = _currentUser.UserId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.TranslationJob.Add(job);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            await _messageSender.SendMessage<PublishTranslationsEvent>(new PublishTranslationsEvent
            {
                JobId = job.Id,
            }, cancellationToken);
            
            response.Data = new PublishTranslationsData
            {
                JobId = job.Id
            };

            _logger.LogInformation("{FunctionName} PublishTranslationsCommand processed successfully for JobId: {JobId}", functionName, job.Id);
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