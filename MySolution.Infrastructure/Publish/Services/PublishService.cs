using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using MySolution.Infrastructure.Common.Helpers;

namespace MySolution.Infrastructure.Publish.Services;

public class PublishService : IPublishService
{
    private readonly ILogger<PublishService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ITranslationGeneratorFactory _translationGeneratorFactory;

    public PublishService(
        ILogger<PublishService> logger,
        IUnitOfWork unitOfWork,
        IAzureBlobService azureBlobService,
        ITranslationGeneratorFactory translationGeneratorFactory)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
        _translationGeneratorFactory = translationGeneratorFactory;
    }

    public async Task<PublishStatistics> PublishAsync(
        Guid projectId,
        Guid publishedBy,
        string? notes,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting publish for project {ProjectId}", projectId);
       
        // Load translations
        var translationKeys =
            await _unitOfWork.TranslationKey.GetPublishedTranslationsByProjectAsync(projectId, cancellationToken);
        var exportData = translationKeys
            .SelectMany(x =>
                x.TranslationValues
                    .Where(v =>
                        v.Status == TranslationStatus.Reviewed ||
                        v.Status == TranslationStatus.Published)).ToList();

        if (!exportData.Any())
        {
            throw new InvalidOperationException("No reviewed translations available for publish.");
        }

        var reviewedCount = exportData.Count(x => x.Status == TranslationStatus.Reviewed);
        _logger.LogInformation($"Number of reviewCount: {reviewedCount}");
        var publishedCount = exportData.Count(x => x.Status == TranslationStatus.Published);
        
        // Generate Package
        var languageLookup = exportData
            .GroupBy(x => x.Language.Code)
            .Select(group => new TranslationExportData
            {
                LanguageCode = group.Key,
                Translations = group.ToDictionary(
                    x => x.TranslationKey.Key,
                    x => x.Value)
            }).ToList();

        _logger.LogInformation("Generating publish package for project {ProjectId}", projectId);

        var generator = _translationGeneratorFactory.GetGenerator(FileType.Json);
        var packageStream = await generator.GenerateAsync(languageLookup, cancellationToken);
        var checksum = PublishChecksumHelper.Calculate(languageLookup);
            
        //Duplicate Release Check:
        _logger.LogWarning("PROJECT {ProjectId} - CHECKSUM = {Checksum}", projectId, checksum);

        var duplicated =
            await _unitOfWork.TranslationRelease.ExistReleaseWithChecksumAsync(projectId, checksum, cancellationToken);
        _logger.LogWarning("Duplicated: {Duplicated}", duplicated);
        if (duplicated)
        {
            _logger.LogInformation("Publish skipped because no changes detected. Project {ProjectId}", projectId);
            return new PublishStatistics
            {
                TotalRecords = exportData.Count,
                SuccessRecords = 0,
                FailedRecords = 0,
                SkippedRecords = exportData.Count
            };
        }
     
        var releaseId = Guid.CreateVersion7();
        var blobName = $"releases/{projectId}/release_{releaseId}.zip";
        string? uploadedBlobName = null;

        try
        {
            // Upload package to Azure Blob
            _logger.LogInformation("Uploading publish package for project {ProjectId}", projectId);

            var fileName = await _azureBlobService.UploadFileAsync(packageStream, blobName, cancellationToken);
            uploadedBlobName = fileName;
            var downloadUrl = _azureBlobService.GetFileUrl(fileName);

            _logger.LogInformation("Publish package uploaded successfully for project {ProjectId}", projectId);
            
            // PHASE 3: Database transaction
            _logger.LogInformation("Opening publish transaction for project {ProjectId}", projectId);
            await _unitOfWork.OpenTransactionAsync(cancellationToken);

            try
            {
                // Deactivate current active releases
                var activeReleases =
                    await _unitOfWork.TranslationRelease.GetActiveByProjectAsync(projectId, cancellationToken);

                foreach (var  x in activeReleases)
                {
                    x.IsActive = false;
                }

                // Calculate next release version
                var latestVersion = await _unitOfWork.TranslationRelease.GetLatestVersionAsync(projectId, cancellationToken);
                
                // test:
                _logger.LogWarning("PROJECT {ProjectId} - LatestVersion = {Version}", projectId, latestVersion);

                /*// TEST ONLY
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);*/
                var nextVersion = latestVersion + 1;
                _logger.LogWarning("PROJECT {ProjectId} - NextVersion = {Version}", projectId, nextVersion);
                
                // Create new release
                var release = new TranslationRelease
                {
                    Id = releaseId,
                    ProjectId = projectId,
                    Version = nextVersion,
                    BlobFileName = blobName,
                    DownloadUrl = downloadUrl,
                    Checksum = checksum,
                    IsActive = true,
                    Notes = notes,
                    PublishedAt = DateTime.UtcNow,
                    PublishedBy = publishedBy,
                    TotalKey = reviewedCount
                };
                
                await _unitOfWork.TranslationRelease.Add(release);
                
                // Reviewed -> Published
                foreach (var item in exportData.Where(x => x.Status == TranslationStatus.Reviewed))
                {
                    item.Status = TranslationStatus.Published;
                    item.PublishedAt = DateTime.UtcNow;
                    item.PublishedBy = publishedBy;
                }
             
                // Save DB changes
                await _unitOfWork.SaveAsync(cancellationToken);
                
                // Commit transaction
                await _unitOfWork.CommitAsync(cancellationToken);
                _logger.LogInformation("Release v{Version} created successfully for project {ProjectId}", nextVersion, projectId);
                
                return new PublishStatistics
                {
                    TotalRecords = exportData.Count,
                    SuccessRecords = reviewedCount,
                    SkippedRecords = publishedCount,
                    FailedRecords = 0,
                    Version = nextVersion,
                    BlobFileName = blobName,
                    DownloadUrl = downloadUrl
                };
            }
            catch (Exception ex)
            {
                // Rollback database transaction
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Database transaction failed while publishing project {ProjectId}", projectId);
                throw;
            }
        }
        catch (Exception ex)
        {
            
            // PHASE 4: Blob compensation
            if (uploadedBlobName is not null)
            {
                try
                {
                    await _azureBlobService.DeleteFileAsync(uploadedBlobName, cancellationToken);
                    _logger.LogInformation("Cleaned up uploaded blob {BlobName} after publish failure", uploadedBlobName);
                }
                catch (Exception cleanupException)
                {
                    // Do not hide the original publish exception.
                    _logger.LogError(cleanupException, "Failed to cleanup blob {BlobName} after publish failure", uploadedBlobName);
                }
            }

            _logger.LogError(ex, "Publish failed for project {ProjectId}", projectId);
            throw;
        }
    }
}

