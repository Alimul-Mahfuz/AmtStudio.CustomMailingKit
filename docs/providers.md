# SMTP Providers Setup

Here are configuration reference settings for leading cloud email services and local development servers:

---

## 1. Google Gmail / Google Workspace

- **Host**: `smtp.gmail.com`
- **Port**: `587` (or `465`)
- **SecurityMode**: `StartTls` (for port 587) or `SslOnConnect` (for port 465)
- **UserName**: Your full Gmail address
- **Password**: Google **App Password** *(Requires 2-Step Verification enabled in your Google Account)*

```json
{
  "SmtpOptions": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SecurityMode": "StartTls",
    "UserName": "your-account@gmail.com",
    "Password": "abcd efgh ijkl mnop",
    "DefaultFromAddress": "your-account@gmail.com",
    "DefaultFromName": "My Google App"
  }
}
```

---

## 2. Microsoft 365 / Exchange Online

- **Host**: `smtp.office365.com`
- **Port**: `587`
- **SecurityMode**: `StartTls`
- **UserName**: Your Office 365 mailbox address
- **Password**: Office 365 password or App Password
- *Note: Ensure SMTP AUTH is enabled for the mailbox in the Microsoft 365 Admin Center.*

---

## 3. SendGrid

- **Host**: `smtp.sendgrid.net`
- **Port**: `587`
- **SecurityMode**: `StartTls`
- **UserName**: `apikey` *(literal string "apikey")*
- **Password**: Your SendGrid API Key (`SG.xxxxxxxxxxxxxxxx`)

---

## 4. Amazon Simple Email Service (SES)

- **Host**: `email-smtp.<region>.amazonaws.com` (e.g. `email-smtp.us-east-1.amazonaws.com`)
- **Port**: `587`
- **SecurityMode**: `StartTls`
- **UserName**: Your AWS SES SMTP User Key
- **Password**: Your AWS SES SMTP Secret Key

---

## 5. Local Development (MailHog / smtp4dev / Papercut)

For local development and integration tests without sending real emails:

```json
{
  "SmtpOptions": {
    "Host": "localhost",
    "Port": 1025,
    "SecurityMode": "None",
    "DefaultFromAddress": "test@localhost"
  }
}
```
