using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Models;
using AmtStudio.CustomMailingKit.Options;
using MimeKit;

namespace AmtStudio.CustomMailingKit.Services;

/// <summary>
/// Default implementation of <see cref="IMimeMessageConverter"/>.
/// </summary>
public sealed class MimeMessageConverter : IMimeMessageConverter
{
    public MimeMessage Convert(EmailMessage message, SmtpOptions options)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(options);

        var mimeMessage = new MimeMessage();

        // 1. Resolve Sender (From)
        var sender = message.From ?? (
            !string.IsNullOrWhiteSpace(options.DefaultFromAddress)
                ? new EmailAddress(options.DefaultFromAddress, options.DefaultFromName)
                : null
        );

        if (sender is null)
        {
            throw new InvalidOperationException("No sender address was specified, and no DefaultFromAddress is configured in SmtpOptions.");
        }

        mimeMessage.From.Add(ToMailboxAddress(sender));

        // 2. Add Recipients
        if (message.To.Count == 0 && message.Cc.Count == 0 && message.Bcc.Count == 0)
        {
            throw new InvalidOperationException("At least one recipient (To, Cc, or Bcc) must be provided.");
        }

        foreach (var to in message.To)
        {
            mimeMessage.To.Add(ToMailboxAddress(to));
        }

        foreach (var cc in message.Cc)
        {
            mimeMessage.Cc.Add(ToMailboxAddress(cc));
        }

        foreach (var bcc in message.Bcc)
        {
            mimeMessage.Bcc.Add(ToMailboxAddress(bcc));
        }

        foreach (var replyTo in message.ReplyTo)
        {
            mimeMessage.ReplyTo.Add(ToMailboxAddress(replyTo));
        }

        // 3. Subject
        mimeMessage.Subject = message.Subject ?? string.Empty;

        // 4. Priority
        mimeMessage.Priority = message.Priority switch
        {
            EmailPriority.High => MessagePriority.Urgent,
            EmailPriority.Low => MessagePriority.NonUrgent,
            _ => MessagePriority.Normal
        };

        mimeMessage.Importance = message.Priority switch
        {
            EmailPriority.High => MessageImportance.High,
            EmailPriority.Low => MessageImportance.Low,
            _ => MessageImportance.Normal
        };

        // 5. Custom Headers
        foreach (var (headerKey, headerVal) in message.Headers)
        {
            mimeMessage.Headers.Add(headerKey, headerVal);
        }

        // 6. Build Body and Attachments
        var builder = new BodyBuilder();

        if (!string.IsNullOrEmpty(message.PlainTextBody))
        {
            builder.TextBody = message.PlainTextBody;
        }

        if (!string.IsNullOrEmpty(message.HtmlBody))
        {
            builder.HtmlBody = message.HtmlBody;
        }

        foreach (var attachment in message.Attachments)
        {
            AttachItem(builder, attachment);
        }

        mimeMessage.Body = builder.ToMessageBody();

        return mimeMessage;
    }

    private static MailboxAddress ToMailboxAddress(EmailAddress address) =>
        new(address.DisplayName, address.Address);

    private static void AttachItem(BodyBuilder builder, EmailAttachment attachment)
    {
        ContentType? parsedContentType = null;
        if (!string.IsNullOrWhiteSpace(attachment.ContentType))
        {
            ContentType.TryParse(attachment.ContentType, out parsedContentType);
        }

        if (attachment.IsInline)
        {
            MimeEntity linkedResource;
            if (attachment.ContentBytes is not null)
            {
                linkedResource = parsedContentType is not null
                    ? builder.LinkedResources.Add(attachment.FileName, attachment.ContentBytes, parsedContentType)
                    : builder.LinkedResources.Add(attachment.FileName, attachment.ContentBytes);
            }
            else if (attachment.ContentStream is not null)
            {
                linkedResource = parsedContentType is not null
                    ? builder.LinkedResources.Add(attachment.FileName, attachment.ContentStream, parsedContentType)
                    : builder.LinkedResources.Add(attachment.FileName, attachment.ContentStream);
            }
            else if (!string.IsNullOrEmpty(attachment.FilePath))
            {
                linkedResource = parsedContentType is not null
                    ? builder.LinkedResources.Add(attachment.FilePath, parsedContentType)
                    : builder.LinkedResources.Add(attachment.FilePath);
            }
            else
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(attachment.ContentId))
            {
                linkedResource.ContentId = attachment.ContentId.Trim('<', '>');
            }
        }
        else
        {
            if (attachment.ContentBytes is not null)
            {
                if (parsedContentType is not null)
                    builder.Attachments.Add(attachment.FileName, attachment.ContentBytes, parsedContentType);
                else
                    builder.Attachments.Add(attachment.FileName, attachment.ContentBytes);
            }
            else if (attachment.ContentStream is not null)
            {
                if (parsedContentType is not null)
                    builder.Attachments.Add(attachment.FileName, attachment.ContentStream, parsedContentType);
                else
                    builder.Attachments.Add(attachment.FileName, attachment.ContentStream);
            }
            else if (!string.IsNullOrEmpty(attachment.FilePath))
            {
                if (parsedContentType is not null)
                    builder.Attachments.Add(attachment.FilePath, parsedContentType);
                else
                    builder.Attachments.Add(attachment.FilePath);
            }
        }
    }
}
