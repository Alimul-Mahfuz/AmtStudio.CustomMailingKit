# Troubleshooting & FAQ

Common errors encountered when sending emails with SMTP and MailKit, and how to resolve them:

---

## 1. `535 5.7.8 Username and Password not accepted` (Gmail)

### Cause:
Google disabled "Less Secure Apps" access. You cannot use your primary Google account password with SMTP.

### Solution:
1. Go to your **Google Account > Security**.
2. Make sure **2-Step Verification** is turned ON.
3. Search for **App passwords** (or go to `https://myaccount.google.com/apppasswords`).
4. Generate a 16-character App Password (e.g. `abcd efgh ijkl mnop`).
5. Use this 16-character password as the `Password` in `SmtpOptions`.

---

## 2. `535 5.7.139 Authentication unsuccessful` (Microsoft 365)

### Cause:
By default, Microsoft disables **SMTP AUTH** on tenants and individual user mailboxes.

### Solution:
1. In the **Microsoft 365 Admin Center**, navigate to **Users > Active users**.
2. Select the sending user account > **Mail** tab > **Email apps**.
3. Check the box for **Authenticated SMTP** and save.
4. If Security Defaults are enabled in Azure AD / Entra ID, use an App Password or exempt the service account.

---

## 3. `SSL Handshake failed: The remote certificate is invalid`

### Cause:
The server's SSL certificate is untrusted, expired, self-signed (common in local test servers like Docker MailHog), or the host name does not match the certificate.

### Solution:
* **For Local Development Only**: Enable certificate bypass in `appsettings.json`:
  ```json
  "SmtpOptions": {
    "AllowInvalidCertificates": true
  }
  ```
* **For Production**: Verify that the `Host` matches the certificate Common Name / SAN and that the system CA trust store has updated root certificates.

---

## 4. `Connection timed out` or `Network is unreachable`

### Cause:
Many cloud hosting providers (e.g., Azure App Services, AWS EC2, DigitalOcean) block outbound **Port 25** by default to prevent spam.

### Solution:
* Switch from Port 25 to **Port 587** (`StartTls`) or **Port 465** (`SslOnConnect`).
* Verify with your cloud provider's network security group / firewall rules that outbound traffic on port 587 is permitted.

---

## 5. `InvalidOperationException: No sender address was specified...`

### Cause:
Neither `.From("sender@domain.com")` was called on the builder, nor was `DefaultFromAddress` configured in `SmtpOptions`.

### Solution:
Set `DefaultFromAddress` in your configuration:
```json
"SmtpOptions": {
  "DefaultFromAddress": "noreply@yourdomain.com",
  "DefaultFromName": "Your Platform"
}
```

---

## 6. How do I test email sending in Unit Tests without sending real emails?

Inject a mock for `ISmtpClientFactory` or `IEmailService`:

```csharp
using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Models;
using Moq;
using Xunit;

public class MyOrderServiceTests
{
    [Fact]
    public async Task ProcessOrder_SendsEmail()
    {
        var mockEmail = new Mock<IEmailService>();
        mockEmail
            .Setup(s => s.SendAsync(It.IsAny<Action<EmailMessageBuilder>>(), default))
            .ReturnsAsync(EmailResult.Success("test-msg-id"));

        var service = new MyOrderService(mockEmail.Object);
        await service.CompleteOrderAsync("user@example.com");

        mockEmail.Verify(s => s.SendAsync(It.IsAny<Action<EmailMessageBuilder>>(), default), Times.Once);
    }
}
```
