<!-- _coverpage.md -->

# AmtStudio.CustomMailingKit <small>v1.0.0</small>

> A robust, modern, and reusable email delivery library for .NET 10+ powered by **MailKit** and **MimeKit**.

<p align="center">
  <span class="badge badge-blue">.NET 10.0</span>
  <span class="badge badge-blue">MailKit 5.0</span>
  <span class="badge badge-green">20/20 Tests Passing</span>
  <span class="badge badge-purple">MIT License</span>
  <span class="badge badge-orange">NuGet Package</span>
</p>

- 🚀 **Dependency Injection First**: Native `.AddCustomMailingKit()` with `IConfiguration` or code options.
- 🛡️ **Resilient Retries**: Automatic exponential backoff for transient network and socket hiccups.
- 🎨 **Fluent Builder**: Chain `.To()`, `.Subject()`, `.HtmlBody()`, `.AttachFile()`, and `.Priority()`.
- 📎 **Rich Attachments**: Memory streams, byte arrays, disk files, and inline CID (`<img src="cid:logo">`).
- ⚡ **RazorLight Friendly**: Zero friction rendering of dynamic `.cshtml` email views.

[Quick Start](getting-started.md)
[GitHub Repository](https://github.com/Alimul-Mahfuz/AmtStudio.CustomMailingKit)
