using AmtStudio.CustomMailingKit.Models;

namespace AmtStudio.CustomMailingKit.Builders;

/// <summary>
/// A fluent builder for constructing <see cref="EmailMessage"/> instances.
/// </summary>
public sealed class EmailMessageBuilder
{
    private readonly EmailMessage _message = new();

    /// <summary>
    /// Sets the sender address and optional display name.
    /// </summary>
    public EmailMessageBuilder From(string address, string? displayName = null)
    {
        _message.From = new EmailAddress(address, displayName);
        return this;
    }

    /// <summary>
    /// Adds a primary recipient.
    /// </summary>
    public EmailMessageBuilder To(string address, string? displayName = null)
    {
        _message.To.Add(new EmailAddress(address, displayName));
        return this;
    }

    /// <summary>
    /// Adds multiple primary recipients.
    /// </summary>
    public EmailMessageBuilder To(IEnumerable<string> addresses)
    {
        foreach (var address in addresses)
        {
            _message.To.Add(new EmailAddress(address));
        }
        return this;
    }

    /// <summary>
    /// Adds a carbon copy recipient.
    /// </summary>
    public EmailMessageBuilder Cc(string address, string? displayName = null)
    {
        _message.Cc.Add(new EmailAddress(address, displayName));
        return this;
    }

    /// <summary>
    /// Adds multiple carbon copy recipients.
    /// </summary>
    public EmailMessageBuilder Cc(IEnumerable<string> addresses)
    {
        foreach (var address in addresses)
        {
            _message.Cc.Add(new EmailAddress(address));
        }
        return this;
    }

    /// <summary>
    /// Adds a blind carbon copy recipient.
    /// </summary>
    public EmailMessageBuilder Bcc(string address, string? displayName = null)
    {
        _message.Bcc.Add(new EmailAddress(address, displayName));
        return this;
    }

    /// <summary>
    /// Adds multiple blind carbon copy recipients.
    /// </summary>
    public EmailMessageBuilder Bcc(IEnumerable<string> addresses)
    {
        foreach (var address in addresses)
        {
            _message.Bcc.Add(new EmailAddress(address));
        }
        return this;
    }

    /// <summary>
    /// Adds a reply-to address.
    /// </summary>
    public EmailMessageBuilder ReplyTo(string address, string? displayName = null)
    {
        _message.ReplyTo.Add(new EmailAddress(address, displayName));
        return this;
    }

    /// <summary>
    /// Sets the email subject.
    /// </summary>
    public EmailMessageBuilder Subject(string subject)
    {
        _message.Subject = subject ?? string.Empty;
        return this;
    }

    /// <summary>
    /// Sets the HTML formatted message body.
    /// </summary>
    public EmailMessageBuilder HtmlBody(string htmlBody)
    {
        _message.HtmlBody = htmlBody;
        return this;
    }

    /// <summary>
    /// Sets the plain text message body.
    /// </summary>
    public EmailMessageBuilder PlainTextBody(string plainText)
    {
        _message.PlainTextBody = plainText;
        return this;
    }

    /// <summary>
    /// Sets message body content with an explicit HTML flag.
    /// </summary>
    public EmailMessageBuilder Body(string content, bool isHtml = true)
    {
        if (isHtml)
        {
            _message.HtmlBody = content;
        }
        else
        {
            _message.PlainTextBody = content;
        }
        return this;
    }

    /// <summary>
    /// Attaches a file from disk.
    /// </summary>
    public EmailMessageBuilder AttachFile(string filePath, string? contentType = null, string? fileName = null)
    {
        _message.Attachments.Add(EmailAttachment.FromFilePath(filePath, contentType, fileName));
        return this;
    }

    /// <summary>
    /// Attaches raw byte content.
    /// </summary>
    public EmailMessageBuilder AttachBytes(byte[] content, string fileName, string? contentType = null)
    {
        _message.Attachments.Add(EmailAttachment.FromBytes(content, fileName, contentType));
        return this;
    }

    /// <summary>
    /// Attaches a stream.
    /// </summary>
    public EmailMessageBuilder AttachStream(Stream stream, string fileName, string? contentType = null)
    {
        _message.Attachments.Add(EmailAttachment.FromStream(stream, fileName, contentType));
        return this;
    }

    /// <summary>
    /// Attaches an inline linked resource (e.g. image for HTML cid:reference).
    /// </summary>
    public EmailMessageBuilder AttachInline(string contentId, byte[] content, string fileName, string? contentType = null)
    {
        _message.Attachments.Add(EmailAttachment.CreateInline(contentId, content, fileName, contentType));
        return this;
    }

    /// <summary>
    /// Attaches an inline linked resource from a stream.
    /// </summary>
    public EmailMessageBuilder AttachInline(string contentId, Stream stream, string fileName, string? contentType = null)
    {
        _message.Attachments.Add(EmailAttachment.CreateInline(contentId, stream, fileName, contentType));
        return this;
    }

    /// <summary>
    /// Attaches an inline linked resource from a local file path.
    /// </summary>
    public EmailMessageBuilder AttachInlineFile(string contentId, string filePath, string? contentType = null)
    {
        _message.Attachments.Add(EmailAttachment.CreateInlineFromFile(contentId, filePath, contentType));
        return this;
    }

    /// <summary>
    /// Sets priority (Low, Normal, High).
    /// </summary>
    public EmailMessageBuilder Priority(EmailPriority priority)
    {
        _message.Priority = priority;
        return this;
    }

    /// <summary>
    /// Adds a custom MIME header.
    /// </summary>
    public EmailMessageBuilder Header(string name, string value)
    {
        _message.Headers[name] = value;
        return this;
    }

    /// <summary>
    /// Returns the built <see cref="EmailMessage"/> instance.
    /// </summary>
    public EmailMessage Build()
    {
        return _message;
    }
}
