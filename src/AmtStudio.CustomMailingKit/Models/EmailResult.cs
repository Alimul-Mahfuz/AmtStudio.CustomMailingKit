namespace AmtStudio.CustomMailingKit.Models;

/// <summary>
/// Represents the result of an email send operation.
/// </summary>
public sealed record EmailResult
{
    /// <summary>
    /// Gets whether the email was successfully sent.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the unique message identifier assigned by the SMTP server or MimeKit if successful.
    /// </summary>
    public string? MessageId { get; init; }

    /// <summary>
    /// Gets the error message if the send operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the exception that occurred if any.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets the total number of attempts made to send the message.
    /// </summary>
    public int Attempts { get; init; } = 1;

    /// <summary>
    /// Gets the timestamp when the result was generated.
    /// </summary>
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;

    public static EmailResult Success(string? messageId = null, int attempts = 1) =>
        new()
        {
            IsSuccess = true,
            MessageId = messageId,
            Attempts = attempts
        };

    public static EmailResult Failure(string errorMessage, Exception? exception = null, int attempts = 1) =>
        new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Exception = exception,
            Attempts = attempts
        };
}
