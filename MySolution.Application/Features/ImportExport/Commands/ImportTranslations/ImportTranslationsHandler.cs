using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.ImportExport.Commands.ImportTranslations;

public class ImportTranslationsHandler : IRequestHandler<ImportTranslationsCommand, ImportTranslationsResponse>
{
    private readonly ILogger<ImportTranslationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageSender _messageSender;
    private readonly IAzureBlobService _azureBlobService;

    public ImportTranslationsHandler
    (
        ILogger<ImportTranslationsHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IMessageSender messageSender,
        IAzureBlobService azureBlobService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _messageSender = messageSender;
        _azureBlobService = azureBlobService;
    }

    #region Implementation of IRequestHandler<in ImportTranslationsCommand, ImportTranslationsResponse>

    public async Task<ImportTranslationsResponse> Handle(ImportTranslationsCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(ImportTranslationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ImportTranslationsResponse();

        try
        {    
            // Validate project and namespace:
            var isProjectNamespaceValid = await _unitOfWork.Namespace.GetByIdAndProjectIdAsync(payload.NamespaceId, payload.ProjectId);
            if (isProjectNamespaceValid == null)
            {
                response.ErrorMessage = "Invalid project or namespace.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            //Check language
            var isLanguageValid = await _unitOfWork.ProjectLanguage.IsLanguageBelongsToProjectAsync(payload.LanguageId, payload.ProjectId);
            if (!isLanguageValid)
            {
                response.ErrorMessage = "Language is not valid or does not belong to the specified project.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var blobFileName =  $"imports/{Guid.NewGuid()}_{payload.File.FileName}";
            await _azureBlobService.UploadFileAsync(payload.File.OpenReadStream(), blobFileName, cancellationToken);
            var job = new TranslationJob
            {
                Id = Guid.CreateVersion7(),
                ProjectId = payload.ProjectId,
                LanguageId = payload.LanguageId,
                NamespaceId = payload.NamespaceId,
                Type = TranslationJobType.Import,
                Status = TranslationJobStatus.Pending,
                FileType = payload.Format,
                BlobFileName = blobFileName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId
            };
            await _unitOfWork.TranslationJob.Add(job);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            await _messageSender.SendMessage<ImportTranslationsEvent>(new ImportTranslationsEvent
            {
                JobId = job.Id,
            }, cancellationToken);
            
            response.Data = new ImportTranslationsData
            {
                JobId = job.Id,
                Status = job.Status,
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