namespace AmtStudio.CustomMailingKit.Options;

/// <summary>
/// Configuration options for SMTP connection and behavior.
/// </summary>
public sealed class SmtpOptions
{
    public const string SectionName = "SmtpOptions";

    /// <summary>
    /// Gets or sets the SMTP host server name or IP (e.g. smtp.gmail.com).
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SMTP port number (defaults to 587).
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Gets or sets the security mode to use when establishing connection.
    /// </summary>
    public SmtpSecurityMode SecurityMode { get; set; } = SmtpSecurityMode.Auto;

    /// <summary>
    /// Gets or sets the username for SMTP authentication.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the password or app-password for SMTP authentication.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the default sender email address when none is provided on the message.
    /// </summary>
    public string? DefaultFromAddress { get; set; }

    /// <summary>
    /// Gets or sets the default sender display name when none is provided on the message.
    /// </summary>
    public string? DefaultFromName { get; set; }

    /// <summary>
    /// Gets or sets the socket timeout in milliseconds. Defaults to 15,000 ms (15 seconds).
    /// </summary>
    public int TimeoutMilliseconds { get; set; } = 15_000;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for transient network failures. Defaults to 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay in milliseconds between retry attempts. Defaults to 1000 ms.
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1_000;

    /// <summary>
    /// Gets or sets whether to accept invalid or self-signed SSL/TLS certificates (USE FOR LOCAL DEV ONLY). Defaults to false.
    /// </summary>
    public bool AllowInvalidCertificates { get; set; } = false;

    /// <summary>
    /// Validates the options settings.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Host))
        {
            throw new InvalidOperationException("SMTP Host cannot be null or empty.");
        }

        if (Port <= 0 || Port > 65535)
        {
            throw new InvalidOperationException($"Invalid SMTP Port '{Port}'. Must be between 1 and 65535.");
        }

        if (MaxRetryAttempts < 1)
        {
            throw new InvalidOperationException("MaxRetryAttempts must be at least 1.");
        }
    }
}
