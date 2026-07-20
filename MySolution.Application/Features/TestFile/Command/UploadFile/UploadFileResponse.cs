using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.TestFile.Command.UploadFile;

public class UploadFileResponse : BaseResponse
{
    public string? BlobUrl { get; set; }
}