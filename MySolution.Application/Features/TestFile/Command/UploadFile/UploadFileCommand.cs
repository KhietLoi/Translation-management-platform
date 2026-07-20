using MediatR;
using Microsoft.AspNetCore.Http;

namespace MySolution.Application.Features.TestFile.Command.UploadFile;

public class UploadFileCommand : IRequest<UploadFileResponse>
{
    public UploadFileResquest Payload { get; set; }

    public UploadFileCommand(IFormFile file)
    {
        Payload = new UploadFileResquest { File = file };
    }
}
