using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;

namespace AmtStudio.CustomMailingKit.Services;

/// <summary>
/// Factory that creates and connects MailKit SMTP clients.
/// </summary>
public sealed class SmtpClientFactory : ISmtpClientFactory
{
    private readonly ILogger<SmtpClientFactory>? _logger;

    public SmtpClientFactory(ILogger<SmtpClientFactory>? logger = null)
    {
        _logger = logger;
    }

    public async Task<ISmtpClient> CreateConnectedClientAsync(SmtpOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        var client = new SmtpClient
        {
            Timeout = options.TimeoutMilliseconds
        };

        if (options.AllowInvalidCertificates)
        {
            _logger?.LogWarning("SSL Certificate validation is disabled. This should ONLY be used in local development.");
            client.ServerCertificateValidationCallback = (_, _, _, _) => true;
        }

        var secureSocketOption = options.SecurityMode switch
        {
            SmtpSecurityMode.SslOnConnect => SecureSocketOptions.SslOnConnect,
            SmtpSecurityMode.StartTls => SecureSocketOptions.StartTls,
            SmtpSecurityMode.StartTlsWhenAvailable => SecureSocketOptions.StartTlsWhenAvailable,
            SmtpSecurityMode.None => SecureSocketOptions.None,
            _ => SecureSocketOptions.Auto
        };

        _logger?.LogDebug("Connecting to SMTP server at {Host}:{Port} using {SecurityMode}...",
            options.Host, options.Port, secureSocketOption);

        await client.ConnectAsync(options.Host, options.Port, secureSocketOption, cancellationToken);

        if (!string.IsNullOrWhiteSpace(options.UserName))
        {
            _logger?.LogDebug("Authenticating SMTP user {UserName}...", options.UserName);
            await client.AuthenticateAsync(options.UserName, options.Password ?? string.Empty, cancellationToken);
        }

        return client;
    }
}
