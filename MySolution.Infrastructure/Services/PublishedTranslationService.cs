using System.IO.Compression;
using System.Text.Json;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Services;

public class PublishedTranslationService : IPublishedTranslationService
{
    private readonly IAzureBlobService  _blobService;

    public PublishedTranslationService(IAzureBlobService blobService)
    {
        _blobService = blobService;
    }
    
    public async Task<Dictionary<string, string>> GetTranslationsAsync(string blobFile, string languageCode, CancellationToken cancellationToken)
    {
        //Load blob file:
        await using var blobStream = await _blobService.DownloadFileAsync(blobFile, cancellationToken);
        if (blobStream == null)
        {
            throw new FileNotFoundException($"Published translation package not found: {blobFile}");
        }
        
        using var archive = new ZipArchive(blobStream, ZipArchiveMode.Read);
        var jsonFileName = $"{languageCode}.json";

        var entry = archive.Entries.FirstOrDefault(x =>
            string.Equals(x.FullName, jsonFileName, StringComparison.OrdinalIgnoreCase));

        if (entry == null)
        {
            throw new FileNotFoundException($"Translation file '{jsonFileName}' was not found in release package.");
        }
        
        await using var entryStream = entry.Open();
        var translations = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(entryStream, cancellationToken: cancellationToken);
        
        return translations ?? new Dictionary<string, string>();
    }
}