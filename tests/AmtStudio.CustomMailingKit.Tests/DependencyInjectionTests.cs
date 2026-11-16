using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Extensions;
using AmtStudio.CustomMailingKit.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AmtStudio.CustomMailingKit.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddCustomMailingKit_WithAction_RegistersAllServices()
    {
        var services = new ServiceCollection();

        services.AddCustomMailingKit(options =>
        {
            options.Host = "smtp.office365.com";
            options.Port = 587;
            options.DefaultFromAddress = "no-reply@amtstudio.com";
        });

        using var provider = services.BuildServiceProvider();

        var emailService = provider.GetService<IEmailService>();
        var factory = provider.GetService<ISmtpClientFactory>();
        var converter = provider.GetService<IMimeMessageConverter>();
        var templateEngine = provider.GetService<IEmailTemplateEngine>();

        Assert.NotNull(emailService);
        Assert.NotNull(factory);
        Assert.NotNull(converter);
        Assert.NotNull(templateEngine);
    }

    [Fact]
    public void AddCustomMailingKit_WithConfiguration_BindsOptionsCorrectly()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["SmtpOptions:Host"] = "smtp.sendgrid.net",
            ["SmtpOptions:Port"] = "465",
            ["SmtpOptions:SecurityMode"] = "SslOnConnect",
            ["SmtpOptions:UserName"] = "apikey",
            ["SmtpOptions:Password"] = "secret-token",
            ["SmtpOptions:DefaultFromAddress"] = "support@amtstudio.com"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();
        services.AddCustomMailingKit(configuration);

        using var provider = services.BuildServiceProvider();

        var emailService = provider.GetService<IEmailService>();
        Assert.NotNull(emailService);
    }
}
