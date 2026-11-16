using AmtStudio.CustomMailingKit.Models;
using AmtStudio.CustomMailingKit.Options;
using MimeKit;

namespace AmtStudio.CustomMailingKit.Abstractions;

/// <summary>
/// Converts high-level <see cref="EmailMessage"/> objects to MimeKit <see cref="MimeMessage"/> representations.
/// </summary>
public interface IMimeMessageConverter
{
    /// <summary>
    /// Converts an <see cref="EmailMessage"/> into a fully populated <see cref="MimeMessage"/>.
    /// </summary>
    /// <param name="message">The source email message.</param>
    /// <param name="options">The configured SMTP options.</param>
    /// <returns>A configured MimeMessage instance ready for sending.</returns>
    MimeMessage Convert(EmailMessage message, SmtpOptions options);
}
