using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Models;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobContainerClient _container;

    public AzureBlobService(IOptions<AzureBlobOptions> options)
    {
        var blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
        _container = blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
    }

    public async Task<string> UploadFileAsync(Stream stream, string fileName, CancellationToken cancellationToken)
    {
        await _container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var blobClient = _container.GetBlobClient(fileName);
        await blobClient.UploadAsync(stream, true, cancellationToken);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken)
    {
        var blobClient = _container.GetBlobClient(fileName);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public async Task<Stream?> DownloadFileAsync(string fileName, CancellationToken cancellationToken)
    {
        var blobClient = _container.GetBlobClient(fileName);
        var response = await blobClient.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToStream();
    }

    public async Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken)
    {
        var blobClient = _container.GetBlobClient(fileName);
        var exists = await blobClient.ExistsAsync(cancellationToken);
        return exists.Value;
    }

    public async Task<List<BlobFile>> GetAllFilesAsync(CancellationToken cancellationToken)
    {
        var result = new List<BlobFile>();

        await foreach (var blobItem in _container.GetBlobsAsync(cancellationToken: cancellationToken))
        {
            var blobClient = _container.GetBlobClient(blobItem.Name);
            result.Add(new BlobFile
            {
                FileName = blobItem.Name,
                BlobUrl = blobClient.Uri.ToString(),
                Size = blobItem.Properties.ContentLength,
                ContentType = blobItem.Properties.ContentType,
                LastModified = blobItem.Properties.LastModified
            });
        }

        return result;
    }
}