using System.Text;
using AmtStudio.CustomMailingKit.Builders;
using AmtStudio.CustomMailingKit.Models;
using Xunit;

namespace AmtStudio.CustomMailingKit.Tests;

public class EmailMessageBuilderTests
{
    [Fact]
    public void Builder_ConstructsCompleteMessage()
    {
        var bytes = Encoding.UTF8.GetBytes("sample content");

        var message = new EmailMessageBuilder()
            .From("admin@amtstudio.com", "Admin")
            .To("user1@example.com", "User 1")
            .To(new[] { "user2@example.com", "user3@example.com" })
            .Cc("manager@example.com")
            .Bcc("archive@example.com")
            .ReplyTo("support@example.com")
            .Subject("Important Notice")
            .HtmlBody("<h1>Important</h1>")
            .PlainTextBody("Important")
            .Priority(EmailPriority.High)
            .Header("X-Tenant-Id", "tenant-123")
            .AttachBytes(bytes, "doc.txt", "text/plain")
            .AttachInline("logoImg", bytes, "logo.png", "image/png")
            .Build();

        Assert.NotNull(message.From);
        Assert.Equal("admin@amtstudio.com", message.From.Address);
        Assert.Equal("Admin", message.From.DisplayName);
        Assert.Equal(3, message.To.Count);
        Assert.Single(message.Cc);
        Assert.Single(message.Bcc);
        Assert.Single(message.ReplyTo);
        Assert.Equal("Important Notice", message.Subject);
        Assert.Equal("<h1>Important</h1>", message.HtmlBody);
        Assert.Equal("Important", message.PlainTextBody);
        Assert.Equal(EmailPriority.High, message.Priority);
        Assert.Equal("tenant-123", message.Headers["X-Tenant-Id"]);
        Assert.Equal(2, message.Attachments.Count);

        var normalAttachment = message.Attachments.First(a => !a.IsInline);
        Assert.Equal("doc.txt", normalAttachment.FileName);
        Assert.False(normalAttachment.IsInline);

        var inlineAttachment = message.Attachments.First(a => a.IsInline);
        Assert.Equal("logoImg", inlineAttachment.ContentId);
        Assert.True(inlineAttachment.IsInline);
    }

    [Fact]
    public void EmailAddress_ImplicitConversion_Works()
    {
        EmailAddress address = "test@example.com";
        Assert.Equal("test@example.com", address.Address);
        Assert.Null(address.DisplayName);
        Assert.Equal("test@example.com", address.ToString());

        var named = new EmailAddress("test@example.com", "Test User");
        Assert.Equal("Test User <test@example.com>", named.ToString());
    }

    [Fact]
    public void EmailAddress_Throws_OnEmpty()
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress(""));
        Assert.Throws<ArgumentException>(() => new EmailAddress("   "));
    }
}
