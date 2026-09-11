using MediatR;

namespace MySolution.Application.Features.Admin.TestFile.Command.DeleteFile;

public class DeleteFileCommand : IRequest<DeleteFileResponse>
{
    public DeleteFileCommand(DeleteFileRequest payload)
    {
        Payload = payload;
    }

    public DeleteFileRequest Payload { get; set; }
}