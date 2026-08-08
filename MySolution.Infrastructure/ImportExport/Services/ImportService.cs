using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using SendGrid.Helpers.Errors.Model;

namespace MySolution.Infrastructure.ImportExport.Services;

public class ImportService : IImportService
{
    private readonly ILogger<ImportService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ITranslationParserFactory _translationParserFactory;

    public ImportService
    (
        ILogger<ImportService> logger,
        IUnitOfWork unitOfWork,
        IAzureBlobService azureBlobService,
        ITranslationParserFactory translationParserFactory
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
        _translationParserFactory = translationParserFactory;
    }

    public async Task ImportAsync
    (
        Guid projectId,
        Guid languageId,
        Guid namespaceId,
        string fileName,
        FileType format,
        CancellationToken cancellationToken
    )
    {
        // Validate project
        var project =
            await _unitOfWork.Project.GetByIdAsync(projectId);

        if (project == null)
        {
            throw new NotFoundException(
                $"Project with ID {projectId} not found.");
        }

        // Validate language
        var language =
            await _unitOfWork.Language.GetByIdAsync(languageId);

        if (language == null)
        {
            throw new NotFoundException(
                $"Language with ID {languageId} not found.");
        }
        var projectNamespace =
            await _unitOfWork.Namespace.GetByIdAsync(namespaceId);
        if (projectNamespace == null)
        {
            throw new NotFoundException(
                $"Namespace with ID {namespaceId} not found.");
        }
        // Download file from blob
        await using var stream =
            await _azureBlobService.DownloadFileAsync(
                fileName,
                cancellationToken);

        if (stream == null)
        {
            throw new NotFoundException(
                $"Unable to download file '{fileName}'.");
        }

        // Get parser
        var parser =
            _translationParserFactory.GetParser(format);

        // Parse file
        var translations =
            await parser.ParseAsync(
                stream,
                cancellationToken);

        if (!translations.Any())
        {
            throw new BadRequestException(
                "Import file contains no translation data.");
        }

        // Save translations
        await SaveTranslationsAsync(
            projectId,
            languageId,
            namespaceId,
            translations,
            cancellationToken);

        _logger.LogInformation(
            "Imported {Count} translations into project {ProjectId}",
            translations.Count,
            projectId);
    }

    private async Task SaveTranslationsAsync
    (
        Guid projectId,
        Guid languageId,
        Guid namespaceId,
        Dictionary<string, string> translations,
        CancellationToken cancellationToken
    )
    {
        var translationKeys =
            await _unitOfWork.TranslationKey
                .GetByProjectAndNamespaceWithTranslationValuesAsync(
                    projectId,
                    namespaceId,
                    cancellationToken);

        var keyLookup =
            translationKeys.ToDictionary(
                x => x.Key,
                x => x);    

        foreach (var item in translations)
        {
            if (string.IsNullOrWhiteSpace(item.Key))
            {
                continue;
            }

            var key = item.Key.Trim();
            var value = item.Value?.Trim() ?? string.Empty;

            if (!keyLookup.TryGetValue(key, out var translationKey))
            {
                translationKey = new TranslationKey
                {
                    Id = Guid.CreateVersion7(),
                    ProjectId = projectId,
                    NamespaceId = namespaceId,
                    Key = key
                };

                await _unitOfWork.TranslationKey.Add(
                    translationKey);

                keyLookup[key] = translationKey;
            }

            var translationValue =
                translationKey.TranslationValues
                    .FirstOrDefault(x =>
                        x.LanguageId == languageId);

            if (translationValue == null)
            {
                translationValue = new TranslationValue
                {
                    Id = Guid.CreateVersion7(),
                    TranslationKeyId = translationKey.Id,
                    LanguageId = languageId,
                    Value = value,

                    Status = TranslationStatus.Draft,

                    CreatedAt = DateTime.UtcNow
                };

                translationKey.TranslationValues
                    .Add(translationValue);

                await _unitOfWork.TranslationValue.Add(
                    translationValue);

                continue;
            }

            translationValue.Value = value;

            translationValue.Status = TranslationStatus.Draft;

            translationValue.ReviewedAt = null;
            translationValue.ReviewedBy = null;

            translationValue.PublishedAt = null;
            translationValue.PublishedBy = null;

            translationValue.RejectionReason = null;

            translationValue.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveAsync(
            cancellationToken);
    }
}