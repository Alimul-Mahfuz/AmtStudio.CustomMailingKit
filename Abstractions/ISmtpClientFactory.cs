using AmtStudio.CustomMailingKit.Options;
using MailKit.Net.Smtp;

namespace AmtStudio.CustomMailingKit.Abstractions;

/// <summary>
/// Factory interface for creating and configuring MailKit SMTP clients.
/// </summary>
public interface ISmtpClientFactory
{
    /// <summary>
    /// Creates and connects a MailKit <see cref="ISmtpClient"/> according to the specified options.
    /// </summary>
    /// <param name="options">SMTP configuration options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A connected and authenticated ISmtpClient instance.</returns>
    Task<ISmtpClient> CreateConnectedClientAsync(SmtpOptions options, CancellationToken cancellationToken = default);
}
