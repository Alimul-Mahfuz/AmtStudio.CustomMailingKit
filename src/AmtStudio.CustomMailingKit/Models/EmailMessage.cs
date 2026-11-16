namespace AmtStudio.CustomMailingKit.Models;

/// <summary>
/// Represents an email message to be sent.
/// </summary>
public sealed class EmailMessage
{
    /// <summary>
    /// Gets or sets the sender address. If null, the default sender configured in options is used.
    /// </summary>
    public EmailAddress? From { get; set; }

    /// <summary>
    /// Gets the list of primary recipients.
    /// </summary>
    public List<EmailAddress> To { get; } = [];

    /// <summary>
    /// Gets the list of carbon copy recipients.
    /// </summary>
    public List<EmailAddress> Cc { get; } = [];

    /// <summary>
    /// Gets the list of blind carbon copy recipients.
    /// </summary>
    public List<EmailAddress> Bcc { get; } = [];

    /// <summary>
    /// Gets the list of reply-to addresses.
    /// </summary>
    public List<EmailAddress> ReplyTo { get; } = [];

    /// <summary>
    /// Gets or sets the subject line.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the HTML formatted body.
    /// </summary>
    public string? HtmlBody { get; set; }

    /// <summary>
    /// Gets or sets the plain text body.
    /// </summary>
    public string? PlainTextBody { get; set; }

    /// <summary>
    /// Gets or sets the priority of the message.
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// Gets the custom mail headers.
    /// </summary>
    public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the list of file and inline attachments.
    /// </summary>
    public List<EmailAttachment> Attachments { get; } = [];
}
