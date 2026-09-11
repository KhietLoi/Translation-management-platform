using MediatR;

namespace MySolution.Application.Features.Admin.TestFile.Command.UploadFile;

public class UploadFileCommand : IRequest<UploadFileResponse>
{
    public UploadFileCommand(UploadFileResquest payload)
    {
        Payload = payload;
    }

    public UploadFileResquest Payload { get; set; }
}