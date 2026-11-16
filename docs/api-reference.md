# API Reference

Detailed technical documentation for all types and interfaces in `AmtStudio.CustomMailingKit`.

---

## 1. `IEmailService`

Namespace: `AmtStudio.CustomMailingKit.Abstractions`

The primary service contract for transmitting emails with MailKit.

```csharp
public interface IEmailService
{
    // Sends a pre-constructed EmailMessage instance
    Task<EmailResult> SendAsync(
        EmailMessage message, 
        CancellationToken cancellationToken = default);

    // Sends an email configured via the fluent builder
    Task<EmailResult> SendAsync(
        Action<EmailMessageBuilder> configure, 
        CancellationToken cancellationToken = default);

    // Renders a template with dynamic model and sends via builder
    Task<EmailResult> SendTemplateAsync(
        string template, 
        object model, 
        Action<EmailMessageBuilder> configure, 
        CancellationToken cancellationToken = default);

    // Renders a template with dynamic model and sends the provided message
    Task<EmailResult> SendTemplateAsync(
        string template, 
        object model, 
        EmailMessage message, 
        CancellationToken cancellationToken = default);
}
```

---

## 2. `EmailResult`

Namespace: `AmtStudio.CustomMailingKit.Models`

Represents the structured outcome of a send operation.

```csharp
public sealed record EmailResult
{
    public bool IsSuccess { get; init; }
    public string? MessageId { get; init; }
    public string? ErrorMessage { get; init; }
    public Exception? Exception { get; init; }
    public int Attempts { get; init; } = 1;
    public DateTimeOffset TimestampUtc { get; init; }
}
```

---

## 3. `EmailMessage`

Namespace: `AmtStudio.CustomMailingKit.Models`

The complete data model representing an email to be sent.

```csharp
public sealed class EmailMessage
{
    public EmailAddress? From { get; set; }
    public List<EmailAddress> To { get; }
    public List<EmailAddress> Cc { get; }
    public List<EmailAddress> Bcc { get; }
    public List<EmailAddress> ReplyTo { get; }
    public string Subject { get; set; }
    public string? HtmlBody { get; set; }
    public string? PlainTextBody { get; set; }
    public EmailPriority Priority { get; set; }
    public Dictionary<string, string> Headers { get; }
    public List<EmailAttachment> Attachments { get; }
}
```

---

## 4. `EmailAddress`

Namespace: `AmtStudio.CustomMailingKit.Models`

Represents an email address with optional display name and implicit string conversions.

```csharp
public sealed record EmailAddress(string Address, string? DisplayName = null)
{
    public static implicit operator EmailAddress(string address) => new(address);
}
```

---

## 5. `EmailAttachment`

Namespace: `AmtStudio.CustomMailingKit.Models`

Represents regular or inline linked file resources.

```csharp
public sealed class EmailAttachment
{
    public string FileName { get; }
    public string? ContentType { get; }
    public byte[]? ContentBytes { get; }
    public Stream? ContentStream { get; }
    public string? FilePath { get; }
    public bool IsInline { get; }
    public string? ContentId { get; }

    public static EmailAttachment FromFilePath(string filePath, string? contentType = null, string? fileName = null);
    public static EmailAttachment FromBytes(byte[] content, string fileName, string? contentType = null);
    public static EmailAttachment FromStream(Stream stream, string fileName, string? contentType = null);
    public static EmailAttachment CreateInline(string contentId, byte[] content, string fileName, string? contentType = null);
    public static EmailAttachment CreateInline(string contentId, Stream stream, string fileName, string? contentType = null);
    public static EmailAttachment CreateInlineFromFile(string contentId, string filePath, string? contentType = null);
}
```

---

## 6. `ServiceCollectionExtensions`

Namespace: `AmtStudio.CustomMailingKit.Extensions`

Extension methods for registering the service into `IServiceCollection`:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMailingKit(
        this IServiceCollection services, 
        Action<SmtpOptions> configureOptions);

    public static IServiceCollection AddCustomMailingKit(
        this IServiceCollection services, 
        IConfiguration configuration, 
        string sectionName = "SmtpOptions");
}
```
