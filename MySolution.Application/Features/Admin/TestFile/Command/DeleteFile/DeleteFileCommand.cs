using MediatR;
using MySolution.Application.Features.Admin.TestFile.Command.DeleteFile;

namespace MySolution.Application.Features.TestFile.Command.DeleteFile;

public class DeleteFileCommand : IRequest<DeleteFileResponse>
{
    public DeleteFileCommand(DeleteFileRequest payload)
    {
        Payload = payload;
    }

    public DeleteFileRequest Payload { get; set; }
}