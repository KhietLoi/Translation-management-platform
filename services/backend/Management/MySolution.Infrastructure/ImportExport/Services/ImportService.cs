using Microsoft.EntityFrameworkCore;
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
        var project = await _unitOfWork.Project
            .GetAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == projectId, cancellationToken);
        if (project == null)
        {
            throw new NotFoundException($"Project with ID {projectId} not found.");
        }

        // Validate language
        var language = await _unitOfWork.Language
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == languageId, cancellationToken);
        if (language == null)
        {
            throw new NotFoundException($"Language with ID {languageId} not found.");
        }

        // Validate namespace
        var projectNamespace = await _unitOfWork.Namespace
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == namespaceId, cancellationToken);
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
        
        var result = await SaveTranslationsAsync(projectId, languageId, namespaceId, translations, cancellationToken);

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

    // Get all existing translation keys with their translation values
    var translationKeys =
        await _unitOfWork.TranslationKey
            .GetAll()
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId && x.NamespaceId == namespaceId)
            .Include(x => x.TranslationValues)
            .ToListAsync(cancellationToken);

    var keyLookup = translationKeys.ToDictionary(x => x.Key, x => x);

    // Get all languages of project once
    var projectLanguages = await _unitOfWork.ProjectLanguage
        .GetAll()
        .AsNoTracking()
        .Where(pl => pl.ProjectId == projectId)
        .ToListAsync(cancellationToken);

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
        if (!keyLookup.TryGetValue(key, out var translationKey))
        {
            // Create new TranslationKey
            translationKey = new TranslationKey
            {
                Id = Guid.CreateVersion7(),
                ProjectId = projectId,
                NamespaceId = namespaceId,
                Key = key,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TranslationKey.Add(translationKey);
            keyLookup[key] = translationKey;
            result.CreatedKeys++;
            
            _logger.LogInformation("Created translation key {Key}", key);
          
            // Create TranslationValue for every project language
            foreach (var projectLanguage in projectLanguages)
            {
                var isImportedLanguage = projectLanguage.LanguageId == languageId;

                var newTranslationValue = new TranslationValue
                {
                    Id = Guid.CreateVersion7(),
                    TranslationKeyId = translationKey.Id,
                    LanguageId = projectLanguage.LanguageId,

                    Value = isImportedLanguage ? value : string.Empty,
                    Status = isImportedLanguage && !string.IsNullOrWhiteSpace(value)
                        ? TranslationStatus.Draft
                        : TranslationStatus.Missing,
                    CreatedAt = DateTime.UtcNow
                };

                translationKey.TranslationValues.Add(newTranslationValue);
                await _unitOfWork.TranslationValue.Add(newTranslationValue);
                
                if (isImportedLanguage)
                {
                    result.CreatedValues++;
                }
            }

            continue;
        }

        foreach (var projectLanguage in projectLanguages)
        {
            var existingTranslationValue = translationKey.TranslationValues
                .FirstOrDefault(x => x.LanguageId == projectLanguage.LanguageId);
            if (existingTranslationValue != null)
            {
                continue;
            }

            var missingTranslationValue = new TranslationValue
            {
                Id = Guid.CreateVersion7(),
                TranslationKeyId = translationKey.Id,
                LanguageId = projectLanguage.LanguageId,
                Value = string.Empty,
                Status = TranslationStatus.Missing,
                CreatedAt = DateTime.UtcNow
            };

            translationKey.TranslationValues.Add(missingTranslationValue);
            await _unitOfWork.TranslationValue.Add(missingTranslationValue);
        }
        
        // 3. Find TranslationValue of imported language
        var translationValue =
            translationKey.TranslationValues.FirstOrDefault(x => x.LanguageId == languageId);

        if (translationValue == null)
        {
            throw new InvalidOperationException($"Translation value for key '{key}' and language '{languageId}' was not created.");
        }

      
        // 4. Do not overwrite protected translations

        if (translationValue.Status == TranslationStatus.Reviewed ||
            translationValue.Status == TranslationStatus.Published ||
            translationValue.Status == TranslationStatus.Translated)
        {
            result.SkippedRecords++;
            _logger.LogWarning("Skipped translation key {Key} because current status is {Status}", key, translationValue.Status);
            continue;
        }

       
        // 5. Update existing TranslationValue
        translationValue.Value = value;
        translationValue.Status = string.IsNullOrWhiteSpace(value) ? TranslationStatus.Missing : TranslationStatus.Draft;
        translationValue.ReviewedAt = null;
        translationValue.ReviewedBy = null;
        translationValue.PublishedAt = null;
        translationValue.PublishedBy = null;
        translationValue.RejectionReason = null;
        translationValue.UpdatedAt = DateTime.UtcNow;
        result.UpdatedValues++;
    }

        await _unitOfWork.SaveAsync(cancellationToken);
        return result;
    }
}