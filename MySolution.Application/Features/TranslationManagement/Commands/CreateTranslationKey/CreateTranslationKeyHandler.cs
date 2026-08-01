using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationKey;

public class CreateTranslationKeyHandler
    : IRequestHandler<CreateTranslationKeyCommand, CreateTranslationKeyResponse>
{
    private readonly ILogger<CreateTranslationKeyHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTranslationKeyHandler(
        ILogger<CreateTranslationKeyHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateTranslationKeyResponse> Handle(
        CreateTranslationKeyCommand request,
        CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var response = new CreateTranslationKeyResponse();
        var functionName = nameof(CreateTranslationKeyHandler);
        var now = DateTime.UtcNow;

        try
        {
            _logger.LogInformation(
                "{FunctionName} Creating translation key {Key}",
                functionName,
                payload.Key);

            // Validate Namespace
            var namespaceEntity = await _unitOfWork.Namespace
                .GetByIdAndProjectIdAsync(
                    payload.NamespaceId,
                    payload.ProjectId);

            if (namespaceEntity is null)
            {
                response.ErrorMessage = "Namespace does not belong to the specified project.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            // Check duplicate key in namespace
            var exists = await _unitOfWork.TranslationKey.ExistsAsync(
                payload.ProjectId,
                payload.NamespaceId,
                payload.Key);

            if (exists)
            {
                response.ErrorMessage = "Translation key already exists.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }

            // Get project languages
            var languageIds = await _unitOfWork.ProjectLanguage
                .GetLanguagesByProjectIdAsync(payload.ProjectId);

            if (languageIds.Count == 0)
            {
                response.ErrorMessage = "Project does not contain any languages.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            // Create TranslationKey
            var translationKey = new Domain.Entities.TranslationKey
            {
                Id = Guid.CreateVersion7(),
                ProjectId = payload.ProjectId,
                NamespaceId = payload.NamespaceId,
                Key = payload.Key,
                Description = payload.Description,
                CreatedAt = now
            };

            // Create TranslationValues
            var translationValues = languageIds
                .Select(languageId => new Domain.Entities.TranslationValue
                {
                    Id = Guid.CreateVersion7(),
                    TranslationKeyId = translationKey.Id,
                    LanguageId =  languageId.Id,
                    Value = string.Empty,
                    Status = TranslationStatus.Missing,
                    CreatedAt = now
                })
                .ToList();

            await _unitOfWork.TranslationKey.Add(translationKey);
            await _unitOfWork.TranslationValue.AddRange(translationValues);
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new CreateTranslationKeyData
            {
                Id = translationKey.Id,
                ProjectId = translationKey.ProjectId,
                NamespaceId = translationKey.NamespaceId,
                Key = translationKey.Key,
                Description = translationKey.Description,
                CreatedAt = translationKey.CreatedAt
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FunctionName} Unexpected error while creating translation key.",
                functionName);

            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }
}