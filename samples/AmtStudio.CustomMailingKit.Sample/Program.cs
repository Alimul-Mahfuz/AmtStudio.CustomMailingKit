using System.Text;
using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Extensions;
using AmtStudio.CustomMailingKit.Models;
using AmtStudio.CustomMailingKit.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("==================================================");
Console.WriteLine("   AmtStudio.CustomMailingKit - Sample Demo");
Console.WriteLine("==================================================");

// 1. Setup Dependency Injection
var services = new ServiceCollection();

// Configure Logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

// Register AmtStudio.CustomMailingKit
services.AddCustomMailingKit(options =>
{
    // Configure your SMTP provider (e.g. Gmail, Outlook, SendGrid, Mailgun, Amazon SES, local MailHog)
    options.Host = "smtp.example.com";
    options.Port = 587;
    options.SecurityMode = SmtpSecurityMode.StartTls;
    options.UserName = "apikey";
    options.Password = "secret-smtp-password";
    options.DefaultFromAddress = "noreply@amtstudio.com";
    options.DefaultFromName = "AmtStudio Notification";
    options.MaxRetryAttempts = 3;
    options.RetryDelayMilliseconds = 1000;
});

using var serviceProvider = services.BuildServiceProvider();
var emailService = serviceProvider.GetRequiredService<IEmailService>();

// 2. Example 1: Fluent email builder with an attachment
Console.WriteLine("\n[Example 1] Building email with Fluent API and attachments...");
var sampleAttachment = Encoding.UTF8.GetBytes("ID,Customer,Total\n101,John Doe,$49.99\n102,Jane Smith,$120.00");

var message = new AmtStudio.CustomMailingKit.Builders.EmailMessageBuilder()
    .To("customer@example.com", "John Doe")
    .Cc("manager@example.com")
    .Subject("Monthly Account Statement")
    .HtmlBody("<p>Dear John,</p><p>Please find attached your monthly statement.</p>")
    .PlainTextBody("Dear John, please find attached your monthly statement.")
    .Priority(EmailPriority.High)
    .AttachBytes(sampleAttachment, "statement-november.csv", "text/csv")
    .Header("X-Billing-Cycle", "2026-11")
    .Build();

Console.WriteLine($"Message prepared: Subject='{message.Subject}', To={string.Join(", ", message.To)}, Attachments={message.Attachments.Count}");

// 3. Example 2: Sending with dynamic template rendering
Console.WriteLine("\n[Example 2] Rendering email with template engine...");
var template = """
    <h2>Welcome to AmtStudio, {{UserName}}!</h2>
    <p>Your workspace <strong>{{WorkspaceName}}</strong> has been provisioned.</p>
    <p>Login URL: <a href="{{LoginUrl}}">{{LoginUrl}}</a></p>
    """;

var templateData = new
{
    UserName = "Alex Mercer",
    WorkspaceName = "Quantum-Dev",
    LoginUrl = "https://amtstudio.com/workspace/quantum-dev"
};

var templateEngine = serviceProvider.GetRequiredService<IEmailTemplateEngine>();
var renderedHtml = templateEngine.Render(template, templateData);

Console.WriteLine("Rendered HTML preview:");
Console.WriteLine("--------------------------------------------------");
Console.WriteLine(renderedHtml);
Console.WriteLine("--------------------------------------------------");

Console.WriteLine("\nSample completed successfully. To send real emails, update Host, Port, and Credentials in SmtpOptions.");
