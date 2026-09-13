using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.User.Commands.DeleteUser;

/// <summary>
///     Handler for deleting a user by its ID.
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly ILogger<DeleteUserHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserHandler(IUnitOfWork unitOfWork, ILogger<DeleteUserHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteUserHandler)}";
        _logger.LogInformation(functionName);
        var response = new DeleteUserResponse();

        try
        {
            var user = await _unitOfWork.User
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
           
            _unitOfWork.User.Delete(user);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new DeleteUserData
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            };
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }
}