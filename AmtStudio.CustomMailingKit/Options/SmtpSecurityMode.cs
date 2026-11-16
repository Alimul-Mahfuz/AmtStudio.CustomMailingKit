namespace AmtStudio.CustomMailingKit.Options;

/// <summary>
/// Security mode for connecting to an SMTP server.
/// </summary>
public enum SmtpSecurityMode
{
    /// <summary>
    /// Automatically determines security based on port and server capabilities.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Explicit SSL/TLS connection immediately upon connecting (typically port 465).
    /// </summary>
    SslOnConnect = 1,

    /// <summary>
    /// Upgrades the connection to TLS via STARTTLS command (typically port 587 or 25).
    /// </summary>
    StartTls = 2,

    /// <summary>
    /// Upgrades to TLS if the server supports it, otherwise falls back to plain text.
    /// </summary>
    StartTlsWhenAvailable = 3,

    /// <summary>
    /// Connects without encryption (recommended only for local test servers like MailHog/smtp4dev).
    /// </summary>
    None = 4
}
