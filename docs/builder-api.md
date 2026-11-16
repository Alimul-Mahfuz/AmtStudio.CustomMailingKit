# Fluent Message Builder API

The `EmailMessageBuilder` provides an expressive, chaining API for constructing complete `EmailMessage` models.

---

## 📋 Method Reference

### Senders & Recipients

| Method | Parameters | Description |
| :--- | :--- | :--- |
| `.From(address, [name])` | `string address, string? displayName` | Sets the sender. If omitted, falls back to `DefaultFromAddress` in options. |
| `.To(address, [name])` | `string address, string? displayName` | Adds a primary recipient. |
| `.To(addresses)` | `IEnumerable<string> addresses` | Adds multiple primary recipients in bulk. |
| `.Cc(address, [name])` | `string address, string? displayName` | Adds a carbon-copy (CC) recipient. |
| `.Cc(addresses)` | `IEnumerable<string> addresses` | Adds multiple CC recipients. |
| `.Bcc(address, [name])` | `string address, string? displayName` | Adds a blind-carbon-copy (BCC) recipient. |
| `.Bcc(addresses)` | `IEnumerable<string> addresses` | Adds multiple BCC recipients. |
| `.ReplyTo(address, [name])` | `string address, string? displayName` | Sets the reply-to destination for replies. |

---

### Subject & Body

| Method | Parameters | Description |
| :--- | :--- | :--- |
| `.Subject(subject)` | `string subject` | Sets the email subject line. |
| `.HtmlBody(html)` | `string htmlBody` | Sets the rich HTML version of the email. |
| `.PlainTextBody(text)`| `string plainText` | Sets the fallback plain text version for clients without HTML rendering. |
| `.Body(content, isHtml)`| `string content, bool isHtml` | Sets the body content with an explicit format boolean flag. |

---

### Priority & Custom Headers

| Method | Parameters | Description |
| :--- | :--- | :--- |
| `.Priority(priority)` | `EmailPriority (Low, Normal, High)` | Sets the message urgency and importance headers. |
| `.Header(name, value)`| `string name, string value` | Adds a custom MIME header (e.g., `X-Tenant-Id`, `X-Campaign-Id`). |

---

### Attachments

| Method | Parameters | Description |
| :--- | :--- | :--- |
| `.AttachFile(path, [type], [name])` | `string path, string? contentType, string? fileName` | Attaches a physical file from the disk. |
| `.AttachBytes(bytes, name, [type])` | `byte[] content, string fileName, string? contentType` | Attaches in-memory raw byte content. |
| `.AttachStream(stream, name, [type])` | `Stream stream, string fileName, string? contentType` | Attaches a readable stream (e.g. from AWS S3 or Blob storage). |
| `.AttachInline(contentId, bytes, name, [type])` | `string contentId, byte[] content, string fileName, string? contentType` | Embeds an inline linked resource referenced in HTML via `cid:contentId`. |
| `.AttachInlineFile(contentId, path, [type])` | `string contentId, string filePath, string? contentType` | Embeds a local file as an inline linked resource. |

---

## 💡 Real-World Examples

### 1. Simple Notification Email
```csharp
var result = await emailService.SendAsync(b => b
    .To("member@domain.com")
    .Subject("Your Weekly Digest")
    .HtmlBody("<p>Here is what happened this week...</p>")
    .PlainTextBody("Here is what happened this week..."));
```

### 2. Multi-Recipient Urgent Alert with Custom Headers
```csharp
var result = await emailService.SendAsync(b => b
    .From("alerts@amtstudio.com", "System Monitor")
    .To(new[] { "devops1@amtstudio.com", "devops2@amtstudio.com" })
    .Cc("team-lead@amtstudio.com")
    .Subject("[P0] Production Server High Latency Alert")
    .HtmlBody("<h2>High Latency Warning</h2><p>Database response time exceeded 2500ms.</p>")
    .Priority(EmailPriority.High)
    .Header("X-Incident-Id", "INC-99042")
    .Header("X-Auto-Generated", "true"));
```
