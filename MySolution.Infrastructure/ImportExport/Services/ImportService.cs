using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
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

    public ImportService(
        ILogger<ImportService> logger,
        IUnitOfWork unitOfWork,
        IAzureBlobService azureBlobService,
        ITranslationParserFactory translationParserFactory)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
        _translationParserFactory = translationParserFactory;
    }

    public async Task<ImportStatistics> ImportAsync(
        Guid projectId,
        Guid languageId,
        Guid namespaceId,
        string fileName,
        FileType format,
        CancellationToken cancellationToken)
    {
        // Validate project
        var project = await _unitOfWork.Project.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with ID {projectId} not found.");
        }

        // Validate language
        var language = await _unitOfWork.Language.GetByIdAsync(languageId);
        if (language == null)
        {
            throw new NotFoundException($"Language with ID {languageId} not found.");
        }

        // Validate namespace
        var projectNamespace = await _unitOfWork.Namespace.GetByIdAsync(namespaceId);
        if (projectNamespace == null)
        {
            throw new NotFoundException($"Namespace with ID {namespaceId} not found.");
        }

        // Download file from Blob Storage
        await using var stream = await _azureBlobService.DownloadFileAsync(fileName, cancellationToken);
        if (stream == null)
        {
            throw new NotFoundException($"Unable to download file '{fileName}'.");
        }

        // Get parser
        var parser = _translationParserFactory.GetParser(format);
        // Parse file
        var translations = await parser.ParseAsync(stream, cancellationToken);

        _logger.LogInformation("Parsed {Count} translations from file {FileName}", translations.Count, fileName);
        
        if (!translations.Any())
        {
            throw new BadRequestException("Import file contains no translation data.");
        }

        // Save translations
        var result =
            await SaveTranslationsAsync(
                projectId,
                languageId,
                namespaceId,
                translations,
                cancellationToken);

        _logger.LogInformation(
            "Import completed for project {ProjectId}. " +
            "Total={Total}, CreatedKeys={CreatedKeys}, " +
            "CreatedValues={CreatedValues}, UpdatedValues={UpdatedValues}, " +
            "Skipped={Skipped}, Failed={Failed}",
            projectId,
            result.TotalRecords,
            result.CreatedKeys,
            result.CreatedValues,
            result.UpdatedValues,
            result.SkippedRecords,
            result.FailedRecords);

        return result;
    }

    private async Task<ImportStatistics> SaveTranslationsAsync(
        Guid projectId,
        Guid languageId,
        Guid namespaceId,
        Dictionary<string, string> translations,
        CancellationToken cancellationToken)
    {
        var result = new ImportStatistics
        {
            TotalRecords = translations.Count
        };

        var translationKeys =
            await _unitOfWork.TranslationKey
                .GetByProjectAndNamespaceWithTranslationValuesAsync(
                    projectId,
                    namespaceId,
                    cancellationToken);

        var keyLookup = translationKeys.ToDictionary(x => x.Key, x => x);

        foreach (var item in translations)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Skip empty key
            if (string.IsNullOrWhiteSpace(item.Key))
            {
                result.SkippedRecords++;
                _logger.LogWarning("Skipped import record because translation key is empty.");
                continue;
            }

            var key = item.Key.Trim();
            var value = item.Value.Trim();
            // Find existing TranslationKey
            if (!keyLookup.TryGetValue(key, out var translationKey))
            {
                // Create new TranslationKey
                translationKey = new TranslationKey
                {
                    Id = Guid.CreateVersion7(),
                    ProjectId = projectId,
                    NamespaceId = namespaceId,
                    Key = key
                };

                await _unitOfWork.TranslationKey.Add(translationKey);
                keyLookup[key] = translationKey;
                result.CreatedKeys++;
                _logger.LogInformation("Created translation key {Key}", key);
            }

            // Find TranslationValue for target language
            var translationValue =
                translationKey.TranslationValues.FirstOrDefault(x => x.LanguageId == languageId);

            // New TranslationValue
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

                translationKey.TranslationValues.Add(translationValue);
                await _unitOfWork.TranslationValue.Add(translationValue);
                result.CreatedValues++;
                _logger.LogInformation("Created translation value for key {Key}, language {LanguageId}", key, languageId);
                continue;
            }

            // Do not overwrite Reviewed / Published/ Translated translation
            if (translationValue.Status == TranslationStatus.Reviewed ||
                translationValue.Status == TranslationStatus.Published||
                translationValue.Status == TranslationStatus.Translated)
            {
                result.SkippedRecords++;
                _logger.LogWarning("Skipped translation key {Key} because current status is {Status}", key, translationValue.Status);
                continue;
            }

            // Update existing Draft / Rejected translation
            translationValue.Value = value;
            translationValue.Status = TranslationStatus.Draft;
            translationValue.ReviewedAt = new DateTime();
            translationValue.ReviewedBy = null;
            translationValue.PublishedAt = null;
            translationValue.PublishedBy = null;
            translationValue.RejectionReason = null;
            translationValue.UpdatedAt = DateTime.UtcNow;
            result.UpdatedValues++;

            _logger.LogInformation("Updated translation value for key {Key}, language {LanguageId}", key, languageId);
        }
        
        await _unitOfWork.SaveAsync(cancellationToken);
        return result;
    }
}