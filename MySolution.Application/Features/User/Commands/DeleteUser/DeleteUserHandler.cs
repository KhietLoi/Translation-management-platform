using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.User.Commands.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand,DeleteUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandler(IUnitOfWork unitOfWork, ILogger<DeleteUserHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteUserHandler)}";
        _logger.LogInformation(functionName);
        var response = new DeleteUserResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };
        
        try
        {
            //Get user:
            var user = await _unitOfWork.User.GetByIdAsync(request.Id);
            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            //Delete user
            _unitOfWork.User.Delete(user);
            await _unitOfWork.SaveAsync(cancellationToken);
            //Return value
            response.Data = new DeleteUserData
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }catch(Exception e)
        { 
            Console.WriteLine(e);
            throw;
        }
        return response;
    }
}