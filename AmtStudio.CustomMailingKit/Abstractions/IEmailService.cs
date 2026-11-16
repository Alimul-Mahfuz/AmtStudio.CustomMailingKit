using AmtStudio.CustomMailingKit.Builders;
using AmtStudio.CustomMailingKit.Models;

namespace AmtStudio.CustomMailingKit.Abstractions;

/// <summary>
/// Service interface for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="message">The email message model.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<EmailResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email configured via the fluent <see cref="EmailMessageBuilder"/>.
    /// </summary>
    /// <param name="configure">The builder action to configure the message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<EmailResult> SendAsync(Action<EmailMessageBuilder> configure, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders a template with a model and sends the resulting email configured via the builder.
    /// </summary>
    /// <param name="template">The email template string containing {{placeholders}}.</param>
    /// <param name="model">The data model object or dictionary for token replacements.</param>
    /// <param name="configure">The builder action to configure recipients, subject, etc.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<EmailResult> SendTemplateAsync(
        string template,
        object model,
        Action<EmailMessageBuilder> configure,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders a template with a model and sends the specified message with the rendered body.
    /// </summary>
    /// <param name="template">The email template string containing {{placeholders}}.</param>
    /// <param name="model">The data model object or dictionary for token replacements.</param>
    /// <param name="message">The target email message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure with error details.</returns>
    Task<EmailResult> SendTemplateAsync(
        string template,
        object model,
        EmailMessage message,
        CancellationToken cancellationToken = default);
}
