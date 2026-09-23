using System.Net;
using AuthService.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Application.Features.Auth.ResetPassword;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace AuthService.UnitTests.Feature.Auth;

public class ResetPasswordHandlerTests
{
    private readonly Mock<ILogger<ResetPasswordHandler>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IPasswordResetTokenService> _tokenServiceMock;

    private readonly Mock<IUserRepository> _userRepositoryMock;

    private readonly ResetPasswordHandler _handler;

    public ResetPasswordHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ResetPasswordHandler>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<IPasswordResetTokenService>();

        _userRepositoryMock = new Mock<IUserRepository>();

        _unitOfWorkMock
            .Setup(x => x.User)
            .Returns(_userRepositoryMock.Object);

        _handler = new ResetPasswordHandler(
            _loggerMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }
    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_Token_Is_Expired()
    {
        // Arrange
        var command = new ResetPasswordCommand(
            new ResetPasswordRequest
            {
                Token = "token",
                NewPassword = "Password@123"
            });

        _tokenServiceMock
            .Setup(x => x.ValidateToken("token"))
            .Returns(new PasswordResetPayload
            {
                UserId = Guid.CreateVersion7(),
                PasswordVersion = 1,
                ExpiredAt = DateTime.UtcNow.AddMinutes(-1)
            });

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Token expired.", response.ErrorMessage);
        _userRepositoryMock.Verify(x => x.GetAll(), Times.Never);
    }
    
    [Fact]
    public async Task Handle_Should_Return_NotFound_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var command = new ResetPasswordCommand(
            new ResetPasswordRequest
            {
                Token = "token",
                NewPassword = "Password@123"
            });
        _tokenServiceMock
            .Setup(x => x.ValidateToken("token"))
            .Returns(new PasswordResetPayload
            {
                UserId = userId,
                PasswordVersion = 1,
                ExpiredAt = DateTime.UtcNow.AddMinutes(30)
            });
        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create<User>());
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("User not found.", response.ErrorMessage);
    }
    
    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_PasswordVersion_Is_Invalid()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = new User
        {
            Id = userId,
            PasswordVersion = 5
        };
        var command = new ResetPasswordCommand(
            new ResetPasswordRequest
            {
                Token = "token",
                NewPassword = "Password@123"
            });
        _tokenServiceMock
            .Setup(x => x.ValidateToken("token"))
            .Returns(new PasswordResetPayload
            {
                UserId = userId,
                PasswordVersion = 1,
                ExpiredAt = DateTime.UtcNow.AddMinutes(30)
            });
        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(user));
        
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Token is invalid.", response.ErrorMessage);
        Assert.Equal(5, user.PasswordVersion);
        Assert.Equal(string.Empty, user.PasswordHash);
    }
    [Theory]
    [InlineData(UserStatus.Active, UserStatus.Active)]
    [InlineData(UserStatus.NonActive, UserStatus.Active)]
    [InlineData(UserStatus.Blocked, UserStatus.Blocked)]
    public async Task Handle_Should_ResetPassword_And_Preserve_Or_Activate_Status(UserStatus initialStatus, UserStatus expectedStatus)
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        var userId = Guid.CreateVersion7();
        var user = new User
        {
            Id = userId,
            PasswordHash = "old-password",
            PasswordVersion = 1,
            Status = initialStatus
        };
        var command = new ResetPasswordCommand(
            new ResetPasswordRequest
            {
                Token = "token",
                NewPassword = "Password@123"
            });
        _tokenServiceMock
            .Setup(x => x.ValidateToken("token"))
            .Returns(new PasswordResetPayload
            {
                UserId = userId,
                PasswordVersion = 1,
                ExpiredAt = DateTime.UtcNow.AddMinutes(30)
            });
        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(user));
        _passwordHasherMock
            .Setup(x => x.HashPassword("Password@123"))
            .Returns("new-password-hash");
        
        // Act
        var response = await _handler.Handle(command, cancellation.Token);

        // Assert
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("new-password-hash", user.PasswordHash);
        Assert.Equal(2, user.PasswordVersion);
        Assert.Equal(expectedStatus, user.Status);
        _passwordHasherMock.Verify(x => x.HashPassword("Password@123"), Times.Once);
        _unitOfWorkMock.Verify(
            x => x.SaveAsync(cancellation.Token),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_Should_Return_InternalServerError_When_Exception_Occurs()
    {
        // Arrange
        var command = new ResetPasswordCommand(
            new ResetPasswordRequest
            {
                Token = "token",
                NewPassword = "Password@123"
            });

        _tokenServiceMock
            .Setup(x => x.ValidateToken(It.IsAny<string>()))
            .Throws(new Exception("Database error"));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);

        Assert.Equal(
            HttpStatusCode.InternalServerError,
            response.StatusCode);
    }
}