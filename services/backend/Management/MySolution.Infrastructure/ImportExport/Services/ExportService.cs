using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Domain.Enums;
using SendGrid.Helpers.Errors.Model;

namespace MySolution.Infrastructure.ImportExport.Services;

public class ExportService : IExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportService> _logger;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ITranslationGeneratorFactory _translationGeneratorFactory;
    
    public ExportService
    (
        IUnitOfWork unitOfWork,
        ILogger<ExportService> logger,
        IAzureBlobService azureBlobService,
        ITranslationGeneratorFactory translationGeneratorFactory
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _azureBlobService = azureBlobService;
        _translationGeneratorFactory = translationGeneratorFactory;
    }
    public async Task<ExportTranslationResult> ExportAsync(Guid projectId, FileType format, CancellationToken cancellationToken)
    {
        //Validate project:
        var project = await _unitOfWork.Project
            .GetAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == projectId, cancellationToken);
        
        if (project == null)
        {
            throw new NotFoundException($"Project with ID {projectId} not found.");
        }
        
        //Get languages:
        var languages = await _unitOfWork.ProjectLanguage
            .GetAll()
            .AsNoTracking()
            .Where(x => x.ProjectId == project.Id)
            .Select(x => x.Language)
            .ToListAsync(cancellationToken);
        if (!languages.Any())
        {
            throw new BadRequestException("Project does not contain any languages.");
        }
        // Get translations:
        var translationKeys = await _unitOfWork.TranslationKey
            .GetAll()
            .AsNoTracking()
            .Where(x => x.ProjectId == project.Id)
            .Include(x => x.TranslationValues)
            .ToListAsync(cancellationToken);
        if (!translationKeys.Any())
        {
            throw new BadRequestException("No translations found.");
        }
        
        var totalRecords = 0;
        var successRecords = 0;
        var skippedRecords = 0;
        var failedRecords = 0;
        
        var exportDataList = new List <TranslationExportData>();

        foreach (var language in languages)
        {
            var exportData = new TranslationExportData
            {
                LanguageCode = language.Code
            };
            foreach (var translationKey in translationKeys)
            {
                var translationValue =
                    translationKey.TranslationValues
                        .FirstOrDefault(x =>
                            x.LanguageId == language.Id);

                if (translationValue == null)
                {
                    skippedRecords++;
                    continue;
                }
                totalRecords++;
                
                try
                {
                    exportData.Translations.Add(translationKey.Key, translationValue.Value);
                    successRecords++;
                }
                catch (Exception ex)
                {
                    failedRecords++;
                    _logger.LogError(ex, "Failed to export translation key {Key} for language {LanguageCode}", translationKey.Key, language.Code);
                }
                
            }
            exportDataList.Add(exportData);
        }
        var generator = _translationGeneratorFactory.GetGenerator(format);
        var stream = await generator.GenerateAsync(
            exportDataList,
            cancellationToken);
        
        stream.Position = 0;
        var fileName = $"export_{project.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
        
        await _azureBlobService.UploadFileAsync(stream, fileName, cancellationToken);
        var dowloadUrl = _azureBlobService.GetFileUrl(fileName);

        _logger.LogInformation("Exported translations for project {ProjectId} to {FileName}", projectId, fileName);
        
        return new ExportTranslationResult
        { 
            DownloadUrl = dowloadUrl,
            FileName = fileName,
            TotalRecords = totalRecords,
            SuccessRecords = successRecords,
            FailedRecords = failedRecords,
            SkippedRecords = skippedRecords
        };

    }
}