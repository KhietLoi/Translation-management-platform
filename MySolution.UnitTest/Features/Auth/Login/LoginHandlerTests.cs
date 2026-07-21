using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.Auth.Login;
using MySolution.Domain.Entities;

namespace MySolution.UnitTest.Features.Auth.Login;

public class LoginHandlerTests 
{
    private readonly Mock<ILogger<LoginHandler>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IHashService> _hashServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _loggerMock = new Mock<ILogger<LoginHandler>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();
        _hashServiceMock = new Mock<IHashService>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        
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
            _hashServiceMock.Object
        );
    }
    // Test:
    [Fact]
    public async Task Handle_Should_Login_Successfully_When_Credentials_Are_Valid()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Username = "admin",
            Email = "admin@gmail.com",
            PasswordHash = "hashed-password",
            PasswordVersion = 1,
            IsActive = true,
            IsEmailVerified = true
        };
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });
        
        _userRepositoryMock
            .Setup(x => x.GetUserWithRolesAsync("admin"))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword("Password@123", "hashed-password"))
            .Returns(true);
        
        _jwtServiceMock
            .Setup(x => x.GenerateJwtToken(user))
            .Returns("access-token");
        _jwtServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        _jwtServiceMock
            .Setup(x => x.GetRefreshTokenExpirationDate())
            .Returns(DateTime.UtcNow.AddDays(7));
        _jwtServiceMock
            .Setup(x => x.GetAccessTokenExpirationDate())
            .Returns(DateTime.UtcNow.AddHours(1));
        _hashServiceMock
            .Setup(x => x.ComputeSha256("refresh-token"))
            .Returns("hashed-refresh-token");
        // Act
        var response = await _handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data); 
        Assert.Equal("access-token", response.Data.AccessToken); 
        Assert.Equal("refresh-token", response.Data.RefreshToken); 
        Assert.Equal("admin@gmail.com", response.Data.Email); 
        Assert.True(response.Data.IsEmailVerified);
        
        _refreshTokenRepositoryMock.Verify( x => x.Add(It.IsAny<RefreshToken>()), Times.Once); 
        _unitOfWorkMock.Verify( x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_User_Not_Found()
    {
        // Arrange
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "nonexistent",
                Password = "Password@123"
            });

        _userRepositoryMock
            .Setup(x => x.GetUserWithRolesAsync("nonexistent"))
            .ReturnsAsync((User)null);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(response.Success);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Username or Password is incorrect.", response.ErrorMessage);
    }
    
    //Check Account status
    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_User_Is_Not_Active()
    {
        //Arrange
        var user = new User
        {
            Username = "admin",
            IsActive = false
        };
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });
        
        _userRepositoryMock
            .Setup(x => x.GetUserWithRolesAsync("admin"))
            .ReturnsAsync(user);
        
        //Act:
        var response = await _handler.Handle(command, CancellationToken.None);
        //Assert:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Account is locked.", response.ErrorMessage);
    }
    
    // Check Password verification
    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_Password_Is_Incorrect()
    {
        //Arrange
        var user = new User
        {
            Username = "admin",
            PasswordHash = "hashed",
            IsActive = true
        };
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "wrong-password"
            });
        _userRepositoryMock.Setup(x => x.GetUserWithRolesAsync("admin")).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword("Password@123", "wrong-password")).Returns(false);
        //Act
        var response = await _handler.Handle(command, CancellationToken.None);
        
        //Assert
        Assert.False(response.Success);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Username or Password is incorrect.", response.ErrorMessage);
    }
    //Check IsEmailVerified
    [Fact]
    public async Task Handle_Should_Return_BadRequest_When_User_Is_Not_Email_Verified()
    {
        //Arrange
        var user = new User
        {
            Username = "admin",
            PasswordHash = "hashed",
            IsActive = true,
            IsEmailVerified = false,
            Email = "admin@gmail.com"
        };
        var command = new LoginCommand(
            new LoginRequest
            {
                Username = "admin",
                Password = "Password@123"
            });
        _userRepositoryMock.Setup(x => x.GetUserWithRolesAsync("admin")).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword("Password@123", "hashed")).Returns(true);
        //Act:
        var response = await _handler.Handle(command, CancellationToken.None);
        //Assert:
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.False(response.Data.IsEmailVerified);
        Assert.Equal("admin@gmail.com", response.Data.Email);
        Assert.Equal("User is not verified.", response.ErrorMessage); 
    }
}