# Attachments & Inline Images

`AmtStudio.CustomMailingKit` handles all standard attachment types as well as embedded linked resources (`cid:`).

---

## 1. File Attachments from Disk

```csharp
await emailService.SendAsync(b => b
    .To("client@example.com")
    .Subject("Your Invoices")
    .HtmlBody("<p>Please find your documents attached.</p>")
    .AttachFile("C:/Invoices/Inv-001.pdf", "application/pdf"));
```

---

## 2. In-Memory Byte Array (`byte[]`)

Useful when generating reports dynamically (e.g. PDF/Excel libraries) without saving to disk:

```csharp
byte[] pdfBytes = GeneratePdfReport();

await emailService.SendAsync(b => b
    .To("client@example.com")
    .Subject("Monthly Report")
    .HtmlBody("<p>Here is your generated report.</p>")
    .AttachBytes(pdfBytes, "MonthlyReport.pdf", "application/pdf"));
```

---

## 3. Stream Attachments

Ideal for large files or streaming from cloud storage (e.g. AWS S3, Azure Blob Storage):

```csharp
using var stream = await s3Client.GetObjectStreamAsync(...);

await emailService.SendAsync(b => b
    .To("client@example.com")
    .Subject("Exported Data")
    .HtmlBody("<p>Your export is ready.</p>")
    .AttachStream(stream, "export.zip", "application/zip"));
```

---

## 4. Inline Images (`<img src="cid:...">`)

To display an image inside an email body without hosting it on an external website, attach it with a **Content-ID**:

```csharp
byte[] logoBytes = File.ReadAllBytes("Assets/logo.png");

await emailService.SendAsync(b => b
    .To("client@example.com")
    .Subject("Welcome to AmtStudio")
    .HtmlBody("""
        <h1>Welcome!</h1>
        <p><img src="cid:companyLogo" alt="AmtStudio Logo" width="180" /></p>
        <p>Thank you for signing up.</p>
        """)
    .AttachInline("companyLogo", logoBytes, "logo.png", "image/png"));
```

> [!NOTE]
> The `contentId` passed to `.AttachInline("companyLogo", ...)` must match the `src="cid:companyLogo"` attribute in your HTML tag.
