using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.TranslationPipeline.Application.Common.Interfaces.File;
using MySolution.TranslationPipeline.Application.Common.Models;
using SendGrid.Helpers.Errors.Model;
using Shared.Enums;

namespace MySolution.TranslationPipeline.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExportService> _logger;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ITranslationGenerator _translationGenerator;
    
    public ExportService
    (
        IUnitOfWork unitOfWork,
        ILogger<ExportService> logger,
        IAzureBlobService azureBlobService,
        ITranslationGenerator translationGenerator
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _azureBlobService = azureBlobService;
        _translationGenerator = translationGenerator;
    }
    public async Task<ExportTranslationResult> ExportAsync(Guid projectId, ExportFileType format, CancellationToken cancellationToken)
    {
        //Validate project:
        var project = await _unitOfWork.Project.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with ID {projectId} not found.");
        }
        
        //Get languages:
        var languages = await _unitOfWork.ProjectLanguage.GetLanguagesByProjectIdAsync(projectId);
        if (!languages.Any())
        {
            throw new BadRequestException("Project does not contain any languages.");
        }
        // Get translations:
        var translationKeys = await _unitOfWork.TranslationKey
            .GetByProjectWithTranslationValuesAsync(projectId, cancellationToken);
        if (!translationKeys.Any())
        {
            throw new BadRequestException("No translations found.");
        }
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
                    continue;
                }
                
                exportData.Translations.Add(translationKey.Key, translationValue.Value);
            }
            
            exportDataList.Add(exportData);
        }
        
        //await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        
        var stream = await _translationGenerator.GenerateAsync(exportDataList, cancellationToken);
        
        stream.Position = 0;
        var fileName = $"export_{project.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
        
        await _azureBlobService.UploadFileAsync(stream, fileName, cancellationToken);
        var dowloadUrl = _azureBlobService.GetFileUrl(fileName);
        _logger.LogInformation("File {FileName} uploaded to Azure Blob Storage. Download URL: {DownloadUrl}", fileName, dowloadUrl);

        return new ExportTranslationResult
        {
            DownloadUrl = dowloadUrl,
            FileName = fileName
        };
    }
}