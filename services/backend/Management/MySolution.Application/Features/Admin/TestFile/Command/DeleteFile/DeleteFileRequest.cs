namespace MySolution.Application.Features.Admin.TestFile.Command.DeleteFile;

public class DeleteFileRequest
{
    public string FileName { get; set; }

    public DeleteFileRequest(string fileName)
    {
        FileName = fileName;
    }
}