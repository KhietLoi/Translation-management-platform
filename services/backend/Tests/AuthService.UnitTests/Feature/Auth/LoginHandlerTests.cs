using System.Net;
using AuthService.UnitTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.Auth.Login;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace AuthService.UnitTests.Feature.Auth;

public class LoginHandlerTests
{
    private readonly Mock<ILogger<LoginHandler>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IHashService> _hashServiceMock;
    private readonly Mock<ISecurityStampService> _securityStampServiceMock;

    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly LoginHandler _handler;
    
    public LoginHandlerTests()
    {
        _loggerMock = new Mock<ILogger<LoginHandler>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();
        _hashServiceMock = new Mock<IHashService>();
        _securityStampServiceMock = new Mock<ISecurityStampService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

        _unitOfWorkMock
            .Setup(x => x.User)
            .Returns(_userRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(x => x.RefreshToken)
            .Returns(_refreshTokenRepositoryMock.Object);

        _handler = new LoginHandler(
            _loggerMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _hashServiceMock.Object,
            _securityStampServiceMock.Object);
    }
    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_User_Not_Found()
    {
        // Arrange
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create<User>());
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        VerifyNoSessionCreated();
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Username or Password is incorrect.", response.ErrorMessage);
    }
    [Fact]
    public async Task Handle_Should_Return_Forbidden_When_User_Is_NonActive()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = "admin",
            Status = UserStatus.NonActive
        };

        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(user));
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        VerifyNoSessionCreated();
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Account is not active.", response.ErrorMessage);

        _passwordHasherMock.Verify(
            x => x.VerifyPassword(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }
    
    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_User_Is_Blocked()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = "admin",
            Status = UserStatus.Blocked
        };

        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(user));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        VerifyNoSessionCreated();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Account is blocked.", response.ErrorMessage);

        _passwordHasherMock.Verify(
            x => x.VerifyPassword(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }
    
    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_Password_Is_Invalid()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = "admin",
            PasswordHash = "hashed-password",
            Status = UserStatus.Active
        };

        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "wrong-password"
            });

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(user));

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "wrong-password",
                "hashed-password"))
            .Returns(false);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        VerifyNoSessionCreated();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Username or Password is incorrect.", response.ErrorMessage);
    }
    
    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_Email_Is_Not_Verified()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = "admin",
            Email = "admin@gmail.com",
            PasswordHash = "hashed-password",
            Status = UserStatus.Active,
            IsEmailVerified = false
        };

        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });

        _userRepositoryMock.Setup(x => x.GetAll()).Returns(AsyncQuery.Create(user));
        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "Password@123",
                "hashed-password"))
            .Returns(true);
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.False(response.Success);
        VerifyNoSessionCreated();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("User is not verified.", response.ErrorMessage);
        Assert.NotNull(response.Data);
        Assert.False(response.Data!.IsEmailVerified);
        Assert.Equal("admin@gmail.com", response.Data.Email);
    }
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Handle_Should_Login_Successfully_When_Credentials_Are_Valid(bool profileCompleted)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = "admin",
            Email = "admin@gmail.com",
            PasswordHash = "hashed-password",
            SecurityStamp = Guid.NewGuid().ToString(),
            Status = UserStatus.Active,
            IsEmailVerified = true,
            Profile = profileCompleted
                ? new UserProfile { FullName = "Test User", PhoneNumber = "0901234567", BirthDate = new DateOnly(2000, 1, 1) }
                : new UserProfile()
        };

        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Returns(AsyncQuery.Create(user));

        _passwordHasherMock
            .Setup(x => x.VerifyPassword(
                "Password@123",
                "hashed-password"))
            .Returns(true);

        var accessExpiresAt = DateTime.UtcNow.AddHours(1);
        var refreshExpiresAt = DateTime.UtcNow.AddDays(7);
        using var cancellation = new CancellationTokenSource();
        string? issuedJti = null;
        _hashServiceMock.Setup(x => x.ComputeHash("refresh-token")).Returns("refresh-token-hash");
        _jwtServiceMock
            .Setup(x => x.GenerateJwtToken(
                It.IsAny<User>(),
                It.IsAny<string>()))
            .Callback<User, string>((_, jti) => issuedJti = jti)
            .Returns("access-token");

        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        _jwtServiceMock
            .Setup(x => x.GetAccessTokenExpirationDate())
            .Returns(accessExpiresAt);

        _jwtServiceMock
            .Setup(x => x.GetRefreshTokenExpirationDate())
            .Returns(refreshExpiresAt);


        // Act
        var response = await _handler.Handle(command, cancellation.Token);
        // Assert
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal("access-token", response.Data!.AccessToken);
        Assert.Equal("refresh-token", response.Data.RefreshToken);
        Assert.Equal("admin@gmail.com", response.Data.Email);
        Assert.True(response.Data.IsEmailVerified);
        Assert.Equal(!profileCompleted, response.Data.NeedCompleteProfile);
        Assert.Equal(accessExpiresAt, response.Data.ExpiresAtAccessToken);
        Assert.True(Guid.TryParse(issuedJti, out _));
        _jwtServiceMock.Verify(x => x.GenerateJwtToken(user, It.IsAny<string>()), Times.Once);
        _hashServiceMock.Verify(x => x.ComputeHash("refresh-token"), Times.Once);
        _securityStampServiceMock.Verify(x => x.SetSecurityStampAsync(user.Id, user.SecurityStamp), Times.Once);
        _refreshTokenRepositoryMock.Verify(
            x => x.Add(It.Is<RefreshToken>(token =>
                token.Id != Guid.Empty && token.UserId == user.Id &&
                token.TokenHash == "refresh-token-hash" && token.Jti == issuedJti &&
                token.ExpiredAt == refreshExpiresAt)),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveAsync(cancellation.Token),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_Return_InternalServerError_When_Exception_Occurs()
    {
        // Arrange
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.GetAll())
            .Throws(new Exception("Database error"));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        VerifyNoSessionCreated();

        Assert.Equal(
            HttpStatusCode.InternalServerError,
            response.StatusCode);

        Assert.Equal(
            "An unexpected error occurred.",
            response.ErrorMessage);
    }
    private void VerifyNoSessionCreated()
    {
        _jwtServiceMock.Verify(x => x.GenerateJwtToken(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        _jwtServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Never);
        _refreshTokenRepositoryMock.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        _securityStampServiceMock.Verify(x => x.SetSecurityStampAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }
}
