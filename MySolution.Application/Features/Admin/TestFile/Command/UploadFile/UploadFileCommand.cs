using MediatR;
using MySolution.Application.Features.Admin.TestFile.Command.UploadFile;

namespace MySolution.Application.Features.TestFile.Command.UploadFile;

public class UploadFileCommand : IRequest<UploadFileResponse>
{
    public UploadFileCommand(UploadFileResquest payload)
    {
        Payload = payload;
    }

    public UploadFileResquest Payload { get; set; }
}