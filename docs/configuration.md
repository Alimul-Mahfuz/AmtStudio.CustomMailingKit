# Configuration & Options

The configuration model `SmtpOptions` controls how `AmtStudio.CustomMailingKit` connects, authenticates, and handles network retries.

## `SmtpOptions` Properties

| Property | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `Host` | `string` | *(Required)* | The SMTP server hostname or IP address. |
| `Port` | `int` | `587` | Port number (usually 587 for STARTTLS, 465 for SSL/TLS, 25 for unencrypted). |
| `SecurityMode` | `SmtpSecurityMode` | `Auto` | Connection security policy. |
| `UserName` | `string?` | `null` | Username or API key for authentication. |
| `Password` | `string?` | `null` | Password or secret token for authentication. |
| `DefaultFromAddress` | `string?` | `null` | Default sender email address when not specified per email. |
| `DefaultFromName` | `string?` | `null` | Default sender display name. |
| `TimeoutMilliseconds` | `int` | `15000` | Socket connection and read/write timeout in milliseconds. |
| `MaxRetryAttempts` | `int` | `3` | Maximum retry attempts for transient network or socket failures. |
| `RetryDelayMilliseconds` | `int` | `1000` | Base delay for exponential backoff between retry attempts. |
| `AllowInvalidCertificates` | `bool` | `false` | Bypasses SSL certificate errors. **Use only in local development!** |

---

## Security Modes (`SmtpSecurityMode`)

```csharp
public enum SmtpSecurityMode
{
    Auto = 0,                   // Auto-selects based on port (e.g. 465 -> SslOnConnect, 587 -> StartTls)
    SslOnConnect = 1,           // Immediate SSL handshake upon connection (Port 465)
    StartTls = 2,               // Explicit STARTTLS upgrade (Port 587 / 25)
    StartTlsWhenAvailable = 3,  // Opportunistic TLS
    None = 4                    // Plain unencrypted (For local dev tools like MailHog)
}
```

---

## Resilient Retry Policy

When sending an email over the network, transient socket errors, timeouts, or temporary DNS hiccups can occur. 

`AmtStudio.CustomMailingKit` includes an automated retry loop:

1. **Attempt 1**: Immediate delivery.
2. If an exception occurs (e.g., `SocketException` or `IOException`), the kit waits:
   $$\text{Delay} = \text{RetryDelayMilliseconds} \times 2^{(\text{attempt} - 1)}$$
   *(e.g., 1000ms $\rightarrow$ 2000ms $\rightarrow$ 4000ms)*
3. **Attempt $N$**: Retries up to `MaxRetryAttempts`.
4. If all attempts fail, returns `EmailResult.Failure(...)` with the inner exception and total attempt count without crashing your application.
