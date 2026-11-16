using System.Text;
using AmtStudio.CustomMailingKit.Builders;
using AmtStudio.CustomMailingKit.Models;
using AmtStudio.CustomMailingKit.Options;
using AmtStudio.CustomMailingKit.Services;
using MimeKit;
using Xunit;

namespace AmtStudio.CustomMailingKit.Tests;

public class MimeMessageConverterTests
{
    private readonly MimeMessageConverter _converter = new();

    [Fact]
    public void Convert_UsesDefaultSender_WhenNotProvidedOnMessage()
    {
        var options = new SmtpOptions
        {
            Host = "smtp.test.com",
            DefaultFromAddress = "noreply@amtstudio.com",
            DefaultFromName = "AmtStudio System"
        };

        var message = new EmailMessageBuilder()
            .To("customer@example.com")
            .Subject("Greetings")
            .PlainTextBody("Hello world!")
            .Build();

        var mime = _converter.Convert(message, options);

        Assert.Single(mime.From);
        var sender = Assert.IsType<MailboxAddress>(mime.From[0]);
        Assert.Equal("noreply@amtstudio.com", sender.Address);
        Assert.Equal("AmtStudio System", sender.Name);
    }

    [Fact]
    public void Convert_PrefersMessageSender_OverDefaultSender()
    {
        var options = new SmtpOptions
        {
            Host = "smtp.test.com",
            DefaultFromAddress = "noreply@amtstudio.com"
        };

        var message = new EmailMessageBuilder()
            .From("custom@amtstudio.com", "Custom Sender")
            .To("customer@example.com")
            .Subject("Custom")
            .PlainTextBody("Hi")
            .Build();

        var mime = _converter.Convert(message, options);

        var sender = Assert.IsType<MailboxAddress>(mime.From[0]);
        Assert.Equal("custom@amtstudio.com", sender.Address);
        Assert.Equal("Custom Sender", sender.Name);
    }

    [Fact]
    public void Convert_Throws_WhenNoSenderAvailable()
    {
        var options = new SmtpOptions { Host = "smtp.test.com" };
        var message = new EmailMessageBuilder()
            .To("customer@example.com")
            .Subject("No sender")
            .PlainTextBody("Hi")
            .Build();

        Assert.Throws<InvalidOperationException>(() => _converter.Convert(message, options));
    }

    [Fact]
    public void Convert_Throws_WhenNoRecipientsProvided()
    {
        var options = new SmtpOptions { Host = "smtp.test.com", DefaultFromAddress = "system@test.com" };
        var message = new EmailMessageBuilder()
            .Subject("No recipients")
            .PlainTextBody("Hi")
            .Build();

        Assert.Throws<InvalidOperationException>(() => _converter.Convert(message, options));
    }

    [Fact]
    public void Convert_SetsPriorityAndCustomHeaders()
    {
        var options = new SmtpOptions { Host = "smtp.test.com", DefaultFromAddress = "system@test.com" };
        var message = new EmailMessageBuilder()
            .To("user@example.com")
            .Subject("Urgent")
            .PlainTextBody("Action required")
            .Priority(EmailPriority.High)
            .Header("X-Company-Tag", "Billing")
            .Build();

        var mime = _converter.Convert(message, options);

        Assert.Equal(MessagePriority.Urgent, mime.Priority);
        Assert.Equal(MessageImportance.High, mime.Importance);
        Assert.Equal("Billing", mime.Headers["X-Company-Tag"]);
    }

    [Fact]
    public void Convert_IncludesAttachmentsAndInlineResources()
    {
        var options = new SmtpOptions { Host = "smtp.test.com", DefaultFromAddress = "system@test.com" };
        var fileBytes = Encoding.UTF8.GetBytes("attachment content");
        var imageBytes = Encoding.UTF8.GetBytes("fake image data");

        var message = new EmailMessageBuilder()
            .To("user@example.com")
            .Subject("With files")
            .HtmlBody("<p>Hello <img src=\"cid:myImage\"></p>")
            .AttachBytes(fileBytes, "data.csv", "text/csv")
            .AttachInline("myImage", imageBytes, "img.png", "image/png")
            .Build();

        var mime = _converter.Convert(message, options);

        Assert.NotNull(mime.Body);
        var multipart = Assert.IsAssignableFrom<Multipart>(mime.Body);
        Assert.NotEmpty(multipart);
    }
}
