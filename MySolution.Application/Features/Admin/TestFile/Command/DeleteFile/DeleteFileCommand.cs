using MediatR;

namespace MySolution.Application.Features.TestFile.Command.DeleteFile;

public class DeleteFileCommand : IRequest<DeleteFileResponse>
{
    public DeleteFileRequest Payload { get; set; }

    public DeleteFileCommand(DeleteFileRequest payload)
    {
        Payload = payload;
    }
}