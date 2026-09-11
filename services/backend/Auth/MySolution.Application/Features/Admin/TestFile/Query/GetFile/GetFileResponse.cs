using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Admin.TestFile.Query.GetFile;

public class GetFileResponse : BaseResponse
{
    public List<BlobFile> Files { get; set; } = [];
}