using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;
using MySolution.Application.Features.User.UserProfile.Commands.UploadAvatar;
using MySolution.Application.Features.User.UserProfile.Queries.GetProfileById;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController (IMediator mediator) : Controller
{
    /// <summary>
    /// Get user profile by user ID
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserProfileById(Guid userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProfileByIdQuery(userId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUserProfile(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateProfileCommand(userId, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Upload user avatar
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadAvatarResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadAvatar(Guid userId, UploadAvatarRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UploadAvatarCommand(request, userId), cancellationToken);
        return  ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}