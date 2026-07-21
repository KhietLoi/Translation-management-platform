using Moq;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Features.Auth.SendVerifyEmail;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.UnitTest.Features.Auth;


public class SendVerifyEmailHandlerTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IApplicationUrlProvider> _urlProviderMock;
    private readonly Mock<ITokenSetting> _tokenSettingMock;
    private readonly SendVerifyEmailHandler _handler;


    public SendVerifyEmailHandlerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _urlProviderMock = new Mock<IApplicationUrlProvider>();
        _tokenSettingMock = new Mock<ITokenSetting>();
        
        _tokenSettingMock
            .Setup(x => x.EmailVerificationExpiryMinutes)
            .Returns(30);
        
        _urlProviderMock
            .Setup(x =>
                x.GetVerifyEmailUrl(
                    It.IsAny<string>()))
            .Returns("https://localhost/verify");

        _handler =
            new SendVerifyEmailHandler(
                _emailServiceMock.Object,
                _urlProviderMock.Object,
                _tokenSettingMock.Object);
    }
    
    [Fact]
    public async Task Handle_Should_Send_Email_When_Request_Is_Valid()
    {
        // Arrange
        var command =
            new SendVerifyEmailCommand
            {
                Message =
                    new SendVerifyEmailEvent
                    {
                        Username = "admin",
                        Email = "khietloi2004@gmail.com",
                        Token = "abc"
                    }
            };
        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailServiceMock.Verify(
            x =>
                x.SendEmailAsync(
                    "khietloi2004@gmail.com",
                    "Verify Your Email",
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}