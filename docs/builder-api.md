# Email Message Builder API

The `EmailMessageBuilder` provides a fluent interface to construct complex emails with minimal code.

## Builder Methods Reference

### Recipients & Sender
```csharp
builder
    .From("sender@domain.com", "Display Name")
    .To("user1@domain.com", "User 1")
    .To(new[] { "user2@domain.com", "user3@domain.com" })
    .Cc("manager@domain.com")
    .Bcc("compliance@domain.com")
    .ReplyTo("support@domain.com");
```

### Subject & Content
```csharp
builder
    .Subject("Account Security Alert")
    .HtmlBody("<h2>Security Notice</h2><p>A new login was detected.</p>")
    .PlainTextBody("Security Notice: A new login was detected.");
```

### Priority
```csharp
builder.Priority(EmailPriority.High); // Options: Low, Normal, High
```

### Custom Headers
```csharp
builder
    .Header("X-Tenant-Id", "tenant-8832")
    .Header("X-Campaign-Id", "black-friday-2026");
```

### Complete Example
```csharp
var result = await emailService.SendAsync(builder => builder
    .From("security@amtstudio.com", "AmtStudio Security")
    .To("admin@example.com")
    .Subject("URGENT: Password Reset Requested")
    .HtmlBody("<p>Click <a href='https://amtstudio.com/reset'>here</a> to reset your password.</p>")
    .Priority(EmailPriority.High)
    .Header("X-Priority", "1"));
```
