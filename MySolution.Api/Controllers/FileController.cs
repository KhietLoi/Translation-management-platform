using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.TestFile.Command.DeleteFile;
using MySolution.Application.Features.TestFile.Command.UploadFile;
using MySolution.Application.Features.TestFile.Query.GetFile;

namespace MySolution.Api.Controllers;

[Route("api/[controller]") ]
[ApiController]
public class FileController (IMediator mediator) : Controller
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UploadFileCommand(file), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.BlobUrl);
    }
    
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFile(DeleteFileRequest req, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteFileCommand(req), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Success);
    }
    [HttpGet]
    public async Task<IActionResult> GetAllFiles(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFileQuery(), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result,  result.Files);
    }
    
}