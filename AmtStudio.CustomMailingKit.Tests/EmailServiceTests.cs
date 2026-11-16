using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Builders;
using AmtStudio.CustomMailingKit.Models;
using AmtStudio.CustomMailingKit.Options;
using AmtStudio.CustomMailingKit.Services;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AmtStudio.CustomMailingKit.Tests;

public class EmailServiceTests
{
    private readonly Mock<ISmtpClientFactory> _factoryMock = new();
    private readonly Mock<ISmtpClient> _clientMock = new();
    private readonly MimeMessageConverter _converter = new();
    private readonly SimpleEmailTemplateEngine _templateEngine = new();

    private readonly SmtpOptions _options = new()
    {
        Host = "smtp.test.com",
        Port = 587,
        DefaultFromAddress = "noreply@amtstudio.com",
        MaxRetryAttempts = 2,
        RetryDelayMilliseconds = 10
    };

    public EmailServiceTests()
    {
        _factoryMock
            .Setup(f => f.CreateConnectedClientAsync(It.IsAny<SmtpOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_clientMock.Object);
    }

    private static SmtpSendResult CreateFakeSendResult()
    {
        return new SmtpSendResult(
            SmtpStatusCode.Ok,
            "250 2.0.0 OK",
            [],
            []);
    }

    [Fact]
    public async Task SendAsync_ReturnsSuccess_WhenSendSucceeds()
    {
        _clientMock
            .Setup(c => c.SendAsync(It.IsAny<ISendRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateFakeSendResult());

        var service = new MailKitEmailService(
            Microsoft.Extensions.Options.Options.Create(_options),
            _factoryMock.Object,
            _converter,
            _templateEngine);

        var result = await service.SendAsync(b => b
            .To("test@example.com")
            .Subject("Welcome")
            .PlainTextBody("Test body"));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Attempts);
        Assert.Null(result.ErrorMessage);

        _clientMock.Verify(c => c.SendAsync(It.IsAny<ISendRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _clientMock.Verify(c => c.DisconnectAsync(true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_RetriesOnTransientFailure_AndSucceeds()
    {
        var calls = 0;

        _clientMock
            .Setup(c => c.SendAsync(It.IsAny<ISendRequest>(), It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                calls++;
                if (calls == 1)
                {
                    throw new System.IO.IOException("Transient network blip");
                }
                return Task.FromResult<SendResult>(CreateFakeSendResult());
            });

        var service = new MailKitEmailService(
            Microsoft.Extensions.Options.Options.Create(_options),
            _factoryMock.Object,
            _converter,
            _templateEngine);

        var result = await service.SendAsync(b => b
            .To("test@example.com")
            .Subject("Test Retry")
            .PlainTextBody("Retry me"));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Attempts);
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task SendAsync_ReturnsFailure_WhenMaxAttemptsExceeded()
    {
        _clientMock
            .Setup(c => c.SendAsync(It.IsAny<ISendRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Net.Sockets.SocketException());

        var service = new MailKitEmailService(
            Microsoft.Extensions.Options.Options.Create(_options),
            _factoryMock.Object,
            _converter,
            _templateEngine);

        var result = await service.SendAsync(b => b
            .To("test@example.com")
            .Subject("Failing email")
            .PlainTextBody("Will fail"));

        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Attempts);
        Assert.NotNull(result.ErrorMessage);
        Assert.NotNull(result.Exception);
    }

    [Fact]
    public async Task SendTemplateAsync_RendersAndSends()
    {
        _clientMock
            .Setup(c => c.SendAsync(It.IsAny<ISendRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateFakeSendResult());

        var service = new MailKitEmailService(
            Microsoft.Extensions.Options.Options.Create(_options),
            _factoryMock.Object,
            _converter,
            _templateEngine);

        var template = "<h1>Hello, {{Name}}!</h1><p>Your OTP is {{Otp}}</p>";
        var model = new { Name = "Tushar", Otp = "449911" };

        var result = await service.SendTemplateAsync(template, model, b => b
            .To("user@example.com")
            .Subject("Your Security Code"));

        Assert.True(result.IsSuccess);
        _clientMock.Verify(c => c.SendAsync(It.Is<ISendRequest>(req =>
            req.Message.HtmlBody != null &&
            req.Message.HtmlBody.Contains("Hello, Tushar!") &&
            req.Message.HtmlBody.Contains("449911")), It.IsAny<CancellationToken>()), Times.Once);
    }
}
