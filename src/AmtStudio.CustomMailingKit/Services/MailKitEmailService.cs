using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Builders;
using AmtStudio.CustomMailingKit.Models;
using AmtStudio.CustomMailingKit.Options;
using MailKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AmtStudio.CustomMailingKit.Services;

/// <summary>
/// Primary email delivery service powered by MailKit and MimeKit with built-in retries and logging.
/// </summary>
public sealed class MailKitEmailService : IEmailService
{
    private readonly SmtpOptions _options;
    private readonly ISmtpClientFactory _clientFactory;
    private readonly IMimeMessageConverter _converter;
    private readonly IEmailTemplateEngine _templateEngine;
    private readonly ILogger<MailKitEmailService>? _logger;

    public MailKitEmailService(
        IOptions<SmtpOptions> options,
        ISmtpClientFactory clientFactory,
        IMimeMessageConverter converter,
        IEmailTemplateEngine templateEngine,
        ILogger<MailKitEmailService>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(clientFactory);
        ArgumentNullException.ThrowIfNull(converter);
        ArgumentNullException.ThrowIfNull(templateEngine);

        _options = options.Value;
        _clientFactory = clientFactory;
        _converter = converter;
        _templateEngine = templateEngine;
        _logger = logger;
    }

    public async Task<EmailResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var mimeMessage = _converter.Convert(message, _options);
        var maxAttempts = Math.Max(1, _options.MaxRetryAttempts);
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger?.LogInformation(
                    "Sending email '{Subject}' to {Recipients} (Attempt {Attempt}/{MaxAttempts})...",
                    mimeMessage.Subject,
                    string.Join(", ", mimeMessage.To),
                    attempt,
                    maxAttempts);

                using var client = await _clientFactory.CreateConnectedClientAsync(_options, cancellationToken);
                var sendRequest = new SendRequest(mimeMessage);
                var response = await client.SendAsync(sendRequest, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                _logger?.LogInformation(
                    "Email '{Subject}' sent successfully. Server response: {Response}",
                    mimeMessage.Subject, response);

                return EmailResult.Success(mimeMessage.MessageId, attempt);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger?.LogWarning("Email delivery was canceled by caller.");
                throw;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger?.LogWarning(ex,
                    "Failed to deliver email '{Subject}' on attempt {Attempt}/{MaxAttempts}: {Message}",
                    mimeMessage.Subject, attempt, maxAttempts, ex.Message);

                if (attempt < maxAttempts)
                {
                    var delay = _options.RetryDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
                    await Task.Delay(delay, cancellationToken);
                }
            }
        }

        var failureMessage = $"Failed to send email after {maxAttempts} attempts. Error: {lastException?.Message}";
        _logger?.LogError(lastException, "{FailureMessage}", failureMessage);

        return EmailResult.Failure(failureMessage, lastException, maxAttempts);
    }

    public Task<EmailResult> SendAsync(Action<EmailMessageBuilder> configure, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new EmailMessageBuilder();
        configure(builder);
        return SendAsync(builder.Build(), cancellationToken);
    }

    public Task<EmailResult> SendTemplateAsync(
        string template,
        object model,
        Action<EmailMessageBuilder> configure,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new EmailMessageBuilder();
        configure(builder);
        var message = builder.Build();

        return SendTemplateAsync(template, model, message, cancellationToken);
    }

    public async Task<EmailResult> SendTemplateAsync(
        string template,
        object model,
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(message);

        var rendered = await _templateEngine.RenderAsync(template, model, cancellationToken);

        // If message already indicated HTML or HtmlBody is targeted, assign to HtmlBody; else plain text
        if (!string.IsNullOrEmpty(message.HtmlBody) || string.IsNullOrEmpty(message.PlainTextBody))
        {
            message.HtmlBody = rendered;
        }
        else
        {
            message.PlainTextBody = rendered;
        }

        return await SendAsync(message, cancellationToken);
    }
}
