using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Admin.TestFile.Command.DeleteFile;
using MySolution.Application.Features.Admin.TestFile.Command.UploadFile;
using MySolution.Application.Features.Admin.TestFile.Query.GetFile;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController(IMediator mediator) : Controller
{
    /// <summary>
    /// Uploads a file to the server and returns the URL of the uploaded file.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UploadFileCommand(new UploadFileResquest(file)), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.BlobUrl);
    }

    /// <summary>
    /// Deletes a file from the server based on the provided file name.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFile(DeleteFileRequest req, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteFileCommand(new DeleteFileRequest(req.FileName)), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Success);
    }

    /// <summary>
    /// Retrieves a list of all files stored on the server.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllFiles(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFileQuery(), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result, result.Files);
    }
    
}