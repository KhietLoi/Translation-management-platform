using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.User.Queries.GetUser;
using MySolution.Application.Features.User.UserProfile.Commands.UpdateProfile;
using MySolution.Application.Features.User.UserProfile.Queries.GetProfileById;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserProfileController (IMediator mediator) : Controller
{
    [HttpGet("{userId:guid}")]
    [Permission(PermissionConstants.User.View)]
    public async Task<IActionResult> GetUserProfileById(Guid userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProfileByIdQuery(userId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{userId:guid}")]
    [Permission(PermissionConstants.User.Update)]
    public async Task<IActionResult> UpdateUserProfile(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateProfileCommand(userId, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}