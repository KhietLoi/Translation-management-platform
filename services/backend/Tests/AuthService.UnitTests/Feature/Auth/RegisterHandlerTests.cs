using System.Net;
using AuthService.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Application.Features.Auth.Register;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using Shared.MassTransit.IntegrationEvents;

namespace AuthService.UnitTests.Feature.Auth;

public class RegisterHandlerTests
{
    private readonly Mock<ILogger<RegisterHandler>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMessageSender> _messageSenderMock;
    private readonly Mock<IEmailVerificationTokenService> _emailVerificationTokenServiceMock;

    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;

    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _loggerMock = new Mock<ILogger<RegisterHandler>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _messageSenderMock = new Mock<IMessageSender>();
        _emailVerificationTokenServiceMock = new Mock<IEmailVerificationTokenService>();

        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();

        _unitOfWorkMock
            .Setup(x => x.User)
            .Returns(_userRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(x => x.Role)
            .Returns(_roleRepositoryMock.Object);

        _handler = new RegisterHandler(
            _loggerMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _messageSenderMock.Object,
            _emailVerificationTokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_Email_Or_Username_Already_Exists()
    {
        // Arrange
        var command = new RegisterCommand(
            new RegisterRequest
            {
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailOrUsernameAsync(
                "admin@gmail.com",
                "admin", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _emailVerificationTokenServiceMock.Verify(x => x.GenerateVerificationToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Username and Email already exist.", response.ErrorMessage);
        _userRepositoryMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Never);
        _messageSenderMock.Verify(
            x => x.SendMessage<SendVerifyEmailEvent>(
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Default_Role_Not_Found()
    {
        // Arrange
        var command = new RegisterCommand(
            new RegisterRequest
            {
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailOrUsernameAsync(
                It.IsAny<string>(),
                It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roleRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create<Role>());

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _emailVerificationTokenServiceMock.Verify(x => x.GenerateVerificationToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        Assert.Equal("Default role not found.", response.ErrorMessage);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        _userRepositoryMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
        _messageSenderMock.Verify(
            x => x.SendMessage<SendVerifyEmailEvent>(
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_Register_Successfully_When_Request_Is_Valid()
    {
        // Arrange
        using var cancellation = new CancellationTokenSource();
        User? createdUser = null;
        SendVerifyEmailEvent? sentEmail = null;
        var role = new Role
        {
            Id = Guid.CreateVersion7(),
            Name = RoleConstants.User
        };

        var command = new RegisterCommand(
            new RegisterRequest
            {
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailOrUsernameAsync(
                command.Payload.Email,
                command.Payload.Username, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _roleRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(role));

        _passwordHasherMock
            .Setup(x => x.HashPassword(command.Payload.Password))
            .Returns("hashed-password");

        _userRepositoryMock
            .Setup(x => x.Add(It.IsAny<User>()))
            .Callback<User>(user => createdUser = user)
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(x => x.SaveAsync(cancellation.Token))
            .Returns(Task.CompletedTask);

        _emailVerificationTokenServiceMock
            .Setup(x => x.GenerateVerificationToken(
                It.IsAny<Guid>(),
                It.IsAny<string>()))
            .Returns("verify-token");

        _messageSenderMock
            .Setup(x => x.SendMessage<SendVerifyEmailEvent>(
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _messageSenderMock
            .Setup(x => x.SendMessage<SendVerifyEmailEvent>(It.IsAny<object>(), cancellation.Token))
            .Callback<object, CancellationToken>((message, _) => sentEmail = Assert.IsType<SendVerifyEmailEvent>(message))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _handler.Handle(command, cancellation.Token);
        // Assert
        Assert.True(response.Success);
        Assert.NotNull(createdUser);
        Assert.NotEqual(Guid.Empty, createdUser.Id);
        Assert.Equal("hashed-password", createdUser.PasswordHash);
        Assert.Equal(UserStatus.Active, createdUser.Status);
        Assert.False(createdUser.IsEmailVerified);
        Assert.Equal(role.Id, Assert.Single(createdUser.UserRoles).RoleId);
        Assert.NotNull(createdUser.Profile);
        Assert.Equal(createdUser.Id, createdUser.Profile.UserId);
        Assert.False(createdUser.Profile.IsCompleted);
        Assert.NotNull(sentEmail);
        Assert.Equal(createdUser.Id, sentEmail.UserId);
        Assert.Equal(createdUser.Username, sentEmail.Username);
        Assert.Equal(createdUser.Email, sentEmail.Email);
        Assert.Equal("verify-token", sentEmail.Token);
        _emailVerificationTokenServiceMock.Verify(x => x.GenerateVerificationToken(createdUser.Id, createdUser.Email), Times.Once);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(createdUser.Id, response.Data.Id);
        Assert.Equal("admin", response.Data.Username);
        Assert.Equal("admin@gmail.com", response.Data.Email);
        _userRepositoryMock.Verify(
            x => x.Add(It.IsAny<User>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveAsync(cancellation.Token),
            Times.Once);

        _messageSenderMock.Verify(
            x => x.SendMessage<SendVerifyEmailEvent>(
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Return_InternalServerError_When_Exception_Occurs()
    {
        // Arrange

        var command = new RegisterCommand(
            new RegisterRequest
            {
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailOrUsernameAsync(
                It.IsAny<string>(),
                It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .Throws(new Exception("Database error"));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _emailVerificationTokenServiceMock.Verify(x => x.GenerateVerificationToken(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("An unexpected error occurred.", response.ErrorMessage);
    }
}