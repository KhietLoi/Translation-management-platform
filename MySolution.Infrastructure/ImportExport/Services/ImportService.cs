using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
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
    public async Task ImportAsync(Guid projectId, string fileName, FileType format, CancellationToken cancellationToken)
    {
        // Validate project
        var project = await _unitOfWork.Project.GetByIdAsync(projectId);
        if (project == null)
        {
            throw new NotFoundException($"Project with ID {projectId} not found.");
        }
        
        // Check file exists in blob
        var exists = await _azureBlobService.FileExistsAsync(fileName, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"File '{fileName}' not found in blob storage.");
        }
        
        // Download file
        var stream = await _azureBlobService.DownloadFileAsync(fileName, cancellationToken);
        if (stream == null)
        {
            throw new NotFoundException($"Unable to download file '{fileName}'.");
        }
        
        // Get parser
        var parser = _translationParserFactory.GetParser(format);
        // Parse file
        var importDataList = await parser.ParseAsync(stream, cancellationToken);
        if (!importDataList.Any())
        {
            throw new BadRequestException("Import file contains no translation data.");
        }
        _logger.LogInformation(
            "Parsed {Count} language files for project {ProjectId}",
            importDataList.Count,
            projectId);
    }
}