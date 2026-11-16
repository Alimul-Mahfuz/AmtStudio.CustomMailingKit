# AmtStudio.CustomMailingKit

A modern, robust, and highly reusable email delivery library for .NET 10+ powered by **MailKit** and **MimeKit**.

Designed with clean architecture, enterprise resiliency, fluent builders, and native `Microsoft.Extensions.DependencyInjection` support.

---

## 🌟 Key Features

- **MailKit 5 & MimeKit Native**: Built on the industry-standard, high-performance email framework for .NET.
- **Dependency Injection & Options Pattern**: Effortlessly configure via `IServiceCollection` using `appsettings.json` or code delegates.
- **Fluent Email Builder**: Clean, readable API (`EmailMessageBuilder`) for assembling complex emails.
- **HTML & Plain Text**: Dual-body support with automatic fallback.
- **Rich Attachments**:
  - File path on disk
  - Byte array (`byte[]`)
  - Stream (`Stream`)
  - Inline resources with Content-ID (`cid:logo`) for embedded images in HTML
- **Built-in Resilient Retry**: Configurable retry attempts with exponential backoff for transient network issues.
- **Lightweight Template Engine**: Fast, zero-dependency token replacement with `{{ Key }}` placeholders supporting POCOs, anonymous objects, and dictionaries.
- **Structured Logging**: Deep integration with `Microsoft.Extensions.Logging` for tracing connection, sending, and error events.
- **Unit Tested & Mock-Friendly**: Decoupled `ISmtpClientFactory`, `IMimeMessageConverter`, and `IEmailTemplateEngine` interfaces for seamless testability.

---

## 📦 Installation

Add the package to your .NET project:

```bash
dotnet add package AmtStudio.CustomMailingKit
```

---

## 🚀 Quick Start

### 1. Register with Dependency Injection

#### Option A: Using `appsettings.json`

In your `appsettings.json`:
```json
{
  "SmtpOptions": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SecurityMode": "StartTls",
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "DefaultFromAddress": "noreply@amtstudio.com",
    "DefaultFromName": "AmtStudio Notifications",
    "TimeoutMilliseconds": 15000,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 1000
  }
}
```

In `Program.cs`:
```csharp
using AmtStudio.CustomMailingKit.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register CustomMailingKit bound to "SmtpOptions" section
builder.Services.AddCustomMailingKit(builder.Configuration);
```

#### Option B: In-Code Configuration

```csharp
builder.Services.AddCustomMailingKit(options =>
{
    options.Host = "smtp.office365.com";
    options.Port = 587;
    options.SecurityMode = SmtpSecurityMode.StartTls;
    options.UserName = "admin@company.com";
    options.Password = "securePassword";
    options.DefaultFromAddress = "no-reply@company.com";
    options.DefaultFromName = "Company Portal";
});
```

---

### 2. Sending Emails

Inject `IEmailService` into your controllers, endpoints, or services:

```csharp
using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Models;

public class NotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendWelcomeEmailAsync(string customerEmail, string customerName)
    {
        var result = await _emailService.SendAsync(builder => builder
            .To(customerEmail, customerName)
            .Subject("Welcome to AmtStudio!")
            .HtmlBody($"<h1>Welcome, {customerName}!</h1><p>We're thrilled to have you onboard.</p>")
            .PlainTextBody($"Welcome, {customerName}! We're thrilled to have you onboard.")
            .Priority(EmailPriority.High));

        if (!result.IsSuccess)
        {
            // Handle error
            Console.WriteLine($"Delivery failed: {result.ErrorMessage}");
        }
    }
}
```

---

## 📎 Adding Attachments & Inline Images

### File & Stream Attachments

```csharp
var result = await _emailService.SendAsync(builder => builder
    .To("client@example.com")
    .Subject("Your Invoices")
    .HtmlBody("<p>Please find attached your invoices.</p>")
    .AttachFile("C:/Invoices/Invoice-101.pdf", "application/pdf")
    .AttachBytes(excelBytes, "QuarterlyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
    .AttachStream(reportStream, "summary.csv", "text/csv"));
```

### Inline Images (`<img src="cid:...">`)

Embed an image inside an HTML body without hosting it on an external URL:

```csharp
var result = await _emailService.SendAsync(builder => builder
    .To("client@example.com")
    .Subject("Company Newsletter")
    .HtmlBody("<h1>AmtStudio Weekly</h1><img src=\"cid:companyLogo\"><p>Here is your update.</p>")
    .AttachInline("companyLogo", logoBytes, "logo.png", "image/png"));
```

---

## 📄 Template Rendering

### Option 1: Using RazorLight in your Client App (Recommended for Rich Razor Templates)

If you use **RazorLight** in your client application, render your Razor `.cshtml` file to an HTML string and pass it directly to `.HtmlBody()`:

```csharp
// 1. Render template with RazorLight in your client
string htmlContent = await razorLightEngine.CompileRenderAsync("EmailTemplates/OrderReceipt.cshtml", orderModel);

// 2. Deliver via AmtStudio.CustomMailingKit
var result = await emailService.SendAsync(builder => builder
    .To(orderModel.CustomerEmail, orderModel.CustomerName)
    .Subject($"Receipt for Order #{orderModel.OrderId}")
    .HtmlBody(htmlContent)
    .AttachBytes(pdfBytes, "receipt.pdf", "application/pdf")
    .Priority(EmailPriority.High));
```

### Option 2: Built-in Lightweight Template Engine (Zero-Dependency Placeholders)

Send emails using the built-in placeholder engine:

```csharp
var template = """
    <h2>Hello, {{FullName}}!</h2>
    <p>Your one-time security code is: <strong>{{OtpCode}}</strong></p>
    <p>This code expires in {{ExpiryMinutes}} minutes.</p>
    """;

var model = new
{
    FullName = "Jane Doe",
    OtpCode = "739201",
    ExpiryMinutes = 10
};

var result = await _emailService.SendTemplateAsync(template, model, builder => builder
    .To("jane.doe@example.com")
    .Subject("Your Verification Code"));
```

---

## 🛡️ Common SMTP Provider Configurations

| Provider | Host | Port | SecurityMode | Notes |
| :--- | :--- | :--- | :--- | :--- |
| **Gmail** | `smtp.gmail.com` | 587 | `StartTls` | Requires Google "App Password" |
| **Microsoft 365** | `smtp.office365.com` | 587 | `StartTls` | SMTP AUTH must be enabled in Exchange |
| **SendGrid** | `smtp.sendgrid.net` | 587 | `StartTls` | UserName is `apikey`, Password is your API Key |
| **Amazon SES** | `email-smtp.us-east-1.amazonaws.com` | 587 | `StartTls` | Use SES SMTP credentials |
| **MailHog / Papercut** | `localhost` | 1025 / 25 | `None` | Local testing tool |

---

## 📦 Packing as a NuGet Package (.nupkg)

To pack the project into a distributable NuGet package:

```bash
dotnet pack AmtStudio.CustomMailingKit -c Release -o ./artifacts
```

This generates:
- `AmtStudio.CustomMailingKit.1.0.0.nupkg`
- `AmtStudio.CustomMailingKit.1.0.0.snupkg` (Debugging symbols)

To publish to NuGet.org:
```bash
dotnet nuget push ./artifacts/AmtStudio.CustomMailingKit.1.0.0.nupkg --api-key <YOUR_NUGET_API_KEY> --source https://api.nuget.org/v3/index.json
```

---

## 🧪 Running Tests

```bash
dotnet test
```

Includes 20 comprehensive unit tests covering:
- Address & attachment models
- Fluent builder mechanics
- MimeMessage conversions & linked resources
- Resilient retry policies with simulated transient network errors
- Dependency injection container registration
- Template token engine
