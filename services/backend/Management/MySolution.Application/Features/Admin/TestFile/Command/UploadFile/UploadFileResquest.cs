using Microsoft.AspNetCore.Http;

namespace MySolution.Application.Features.Admin.TestFile.Command.UploadFile;

public class UploadFileResquest
{
    public IFormFile File { get; init; } 

    public UploadFileResquest(IFormFile file)
    {
        File = file;
    }
}