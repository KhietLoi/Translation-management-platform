using Microsoft.AspNetCore.Http;

namespace MySolution.Application.Features.TestFile.Command.UploadFile;

public class UploadFileResquest
{
    public IFormFile File { get; init; }
}