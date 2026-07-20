using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces;

public interface IAzureBlobService
{
    Task<string> UploadFileAsync(Stream stream, string fileName, CancellationToken cancellationToken);
    Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken);
    Task <Stream?> DownloadFileAsync(string fileName, CancellationToken cancellationToken);
    Task<bool> FileExistsAsync(string fileName, CancellationToken cancellationToken);
    Task<List<BlobFile>> GetAllFilesAsync(CancellationToken cancellationToken);
    
}