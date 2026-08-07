namespace MySolution.TranslationPipeline.Application.Common.Models;

public class BlobFile
{
    public string FileName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public long? Size { get; set; }
    public string? ContentType { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}