using System.IO.Compression;
using System.Text.Json;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.File;
using MySolution.Application.Common.Models;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Publish.Services;

public class ReleaseDiffService : IReleaseDiffService
{
    private readonly IAzureBlobService _azureBlobService;

    public ReleaseDiffService(IAzureBlobService azureBlobService)
    {
        _azureBlobService = azureBlobService;
        
    }
    public async Task<ReleaseDiffResult> CompareAsync(TranslationRelease sourceRelease, TranslationRelease targetRelease,
        CancellationToken cancellationToken)
    {
     
        var sourceStream = await _azureBlobService.DownloadFileAsync(sourceRelease.BlobFileName, cancellationToken);
        if (sourceStream == null)
        {
            throw new FileNotFoundException($"Blob {sourceRelease.BlobFileName} not found.");
        }

        var targetStream = await _azureBlobService.DownloadFileAsync(targetRelease.BlobFileName, cancellationToken);
        if (targetStream == null)
        {
            throw new FileNotFoundException($"Blob {targetRelease.BlobFileName} not found.");
        }
        
        var sourceData = await LoadPackageAsync(sourceStream, cancellationToken);
        var targetData = await LoadPackageAsync(targetStream, cancellationToken);
        
        return Compare(sourceData, targetData);
    }
    private async Task<Dictionary<string, Dictionary<string, string>>> LoadPackageAsync
    (
        Stream stream,
        CancellationToken cancellationToken
    )
    {
        var result = new Dictionary<string, Dictionary<string, string>>();
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        
        foreach (var entry in archive.Entries)
        {
            if (!entry.Name.EndsWith(".json")) continue;
         
            await using var entryStream = entry.Open();
            var translations = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>
            (
                entryStream,
                cancellationToken: cancellationToken
            );

            var languageCode = Path.GetFileNameWithoutExtension(entry.Name);
            result[languageCode] = translations ?? [];
        }
        
        return result;
    }
    
     private static ReleaseDiffResult Compare
     (
        Dictionary<string, Dictionary<string, string>> source,
        Dictionary<string, Dictionary<string, string>> target
     )
    {
        var result = new ReleaseDiffResult();
        var languages = source.Keys.Union(target.Keys);

        foreach (var language in languages)
        {
            var oldTranslations = source.GetValueOrDefault(language) ?? [];
            var newTranslations = target.GetValueOrDefault(language) ?? [];
            var keys = oldTranslations.Keys.Union(newTranslations.Keys);

            foreach (var key in keys)
            {
                var oldExists = oldTranslations.TryGetValue(key, out var oldValue);
                var newExists = newTranslations.TryGetValue(key,out var newValue);
                if (!oldExists && newExists)
                {
                    result.Added.Add(new ReleaseDiffItem
                        {
                            LanguageCode = language,
                            Key = key,
                            NewValue = newValue
                        });
                    
                    continue;
                }

                if (oldExists && !newExists)
                {
                    result.Removed.Add(
                        new ReleaseDiffItem
                        {
                            LanguageCode = language,
                            Key = key,
                            OldValue = oldValue
                        });

                    continue;
                }

                if (oldValue != newValue)
                {
                    result.Updated.Add(
                        new ReleaseDiffItem
                        {
                            LanguageCode = language,
                            Key = key,
                            OldValue = oldValue,
                            NewValue = newValue
                        });
                }
            }
        }

        result.AddedCount = result.Added.Count;
        result.UpdatedCount = result.Updated.Count;
        result.RemovedCount = result.Removed.Count;

        return result;
    }
}