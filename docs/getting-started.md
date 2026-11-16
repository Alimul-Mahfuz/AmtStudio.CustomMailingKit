# Quick Start

Follow this guide to install, configure, and send your first email using **AmtStudio.CustomMailingKit** in under 3 minutes.

---

## 1. Installation

Choose your preferred installation method:

<!-- tabs:start -->

#### **.NET CLI**

```bash
dotnet add package AmtStudio.CustomMailingKit
```

#### **Package Manager Console**

```powershell
Install-Package AmtStudio.CustomMailingKit
```

#### **PackageReference (.csproj)**

```xml
<PackageReference Include="AmtStudio.CustomMailingKit" Version="1.0.0" />
```

<!-- tabs:end -->

---

## 2. Register in Dependency Injection (`Program.cs`)

You can register the service using either your `appsettings.json` file or an inline lambda:

<!-- tabs:start -->

#### **Option A: `appsettings.json` (Recommended)**

Add the `SmtpOptions` section to your `appsettings.json`:

```json
{
  "SmtpOptions": {
    "Host": "smtp.sendgrid.net",
    "Port": 587,
    "SecurityMode": "StartTls",
    "UserName": "apikey",
    "Password": "YOUR_SENDGRID_API_KEY",
    "DefaultFromAddress": "noreply@yourdomain.com",
    "DefaultFromName": "Your Platform Name",
    "TimeoutMilliseconds": 15000,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 1000
  }
}
```

Then in `Program.cs`:

```csharp
using AmtStudio.CustomMailingKit.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Binds automatically to the "SmtpOptions" configuration section
builder.Services.AddCustomMailingKit(builder.Configuration);

var app = builder.Build();
app.Run();
```

#### **Option B: In-Code Delegate**

```csharp
using AmtStudio.CustomMailingKit.Extensions;
using AmtStudio.CustomMailingKit.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomMailingKit(options =>
{
    options.Host = "smtp.office365.com";
    options.Port = 587;
    options.SecurityMode = SmtpSecurityMode.StartTls;
    options.UserName = "admin@company.com";
    options.Password = "MySecurePassword";
    options.DefaultFromAddress = "no-reply@company.com";
    options.DefaultFromName = "Company Portal";
    options.MaxRetryAttempts = 3;
});

var app = builder.Build();
app.Run();
```

<!-- tabs:end -->

---

## 3. Inject & Send

Inject `IEmailService` anywhere in your application (Controllers, Minimal APIs, or Services):

```csharp
using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(IEmailService emailService) : ControllerBase
{
    [HttpPost("send-welcome")]
    public async Task<IActionResult> SendWelcome([FromQuery] string email, [FromQuery] string name)
    {
        var result = await emailService.SendAsync(builder => builder
            .To(email, name)
            .Subject("Welcome to our Platform!")
            .HtmlBody($"<h1>Welcome, {name}!</h1><p>We're thrilled to have you onboard.</p>")
            .PlainTextBody($"Welcome, {name}! We're thrilled to have you onboard.")
            .Priority(EmailPriority.High));

        if (result.IsSuccess)
        {
            return Ok(new
            {
                Success = true,
                result.MessageId,
                result.Attempts
            });
        }

        return StatusCode(500, new
        {
            Success = false,
            result.ErrorMessage
        });
    }
}
```

> [!TIP]
> The `builder => builder.To(...)` syntax uses the C# lambda configuration pattern. You don't have to instantiate any message objects manually!
