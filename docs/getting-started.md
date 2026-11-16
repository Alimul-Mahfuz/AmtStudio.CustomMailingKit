# Getting Started

## Installation

Install the package via the .NET CLI:

```bash
dotnet add package AmtStudio.CustomMailingKit
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package AmtStudio.CustomMailingKit
```

---

## 1. Register Services in `Program.cs`

You can register the mailing kit using either `appsettings.json` or inline code configuration:

### Method A: Using `appsettings.json` (Recommended)

In `appsettings.json`:
```json
{
  "SmtpOptions": {
    "Host": "smtp.sendgrid.net",
    "Port": 587,
    "SecurityMode": "StartTls",
    "UserName": "apikey",
    "Password": "YOUR_SENDGRID_KEY",
    "DefaultFromAddress": "noreply@yourdomain.com",
    "DefaultFromName": "Your Company Portal",
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

// Automatically binds to "SmtpOptions" configuration section
builder.Services.AddCustomMailingKit(builder.Configuration);

var app = builder.Build();
app.Run();
```

---

### Method B: In-Code Configuration

```csharp
using AmtStudio.CustomMailingKit.Extensions;
using AmtStudio.CustomMailingKit.Options;

builder.Services.AddCustomMailingKit(options =>
{
    options.Host = "smtp.office365.com";
    options.Port = 587;
    options.SecurityMode = SmtpSecurityMode.StartTls;
    options.UserName = "admin@company.com";
    options.Password = "Password123";
    options.DefaultFromAddress = "noreply@company.com";
    options.DefaultFromName = "Company Notifications";
});
```

---

## 2. Send Your First Email

Inject `IEmailService` into any controller, minimal API endpoint, or background service:

```csharp
using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestEmailController(IEmailService emailService) : ControllerBase
{
    [HttpPost("send-test")]
    public async Task<IActionResult> SendTestEmail([FromQuery] string recipient)
    {
        var result = await emailService.SendAsync(builder => builder
            .To(recipient)
            .Subject("Hello from AmtStudio.CustomMailingKit!")
            .HtmlBody("<h1>Success!</h1><p>Your email system is fully operational.</p>")
            .PlainTextBody("Success! Your email system is fully operational.")
            .Priority(EmailPriority.Normal));

        if (result.IsSuccess)
        {
            return Ok(new { result.MessageId, result.Attempts });
        }

        return BadRequest(new { Error = result.ErrorMessage });
    }
}
```
