using AmtStudio.CustomMailingKit.Abstractions;
using AmtStudio.CustomMailingKit.Options;
using AmtStudio.CustomMailingKit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AmtStudio.CustomMailingKit.Extensions;

/// <summary>
/// Extension methods for registering <see cref="IEmailService"/> and related services into the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the CustomMailingKit email services configured via an <see cref="Action{SmtpOptions}"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Delegate to configure <see cref="SmtpOptions"/>.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddCustomMailingKit(
        this IServiceCollection services,
        Action<SmtpOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        RegisterCoreServices(services);

        return services;
    }

    /// <summary>
    /// Registers the CustomMailingKit email services bound to an <see cref="IConfiguration"/> section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <param name="sectionName">The configuration section name (defaults to "SmtpOptions").</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddCustomMailingKit(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = SmtpOptions.SectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);
        services.Configure<SmtpOptions>(section);
        RegisterCoreServices(services);

        return services;
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.TryAddTransient<ISmtpClientFactory, SmtpClientFactory>();
        services.TryAddSingleton<IMimeMessageConverter, MimeMessageConverter>();
        services.TryAddSingleton<IEmailTemplateEngine, SimpleEmailTemplateEngine>();
        services.TryAddTransient<IEmailService, MailKitEmailService>();
    }
}
