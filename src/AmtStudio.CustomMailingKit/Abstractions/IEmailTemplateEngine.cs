namespace AmtStudio.CustomMailingKit.Abstractions;

/// <summary>
/// Defines an engine for rendering templates with dynamic models (e.g. Simple placeholders, RazorLight, Fluid).
/// </summary>
public interface IEmailTemplateEngine
{
    /// <summary>
    /// Synchronously renders a template string with model values.
    /// </summary>
    /// <param name="template">The raw template string or template key.</param>
    /// <param name="model">The model containing replacement data.</param>
    /// <returns>The rendered HTML or plain text string.</returns>
    string Render(string template, object model) =>
        RenderAsync(template, model).GetAwaiter().GetResult();

    /// <summary>
    /// Asynchronously renders a template with model values.
    /// </summary>
    /// <param name="template">The raw template string, file path, or template key.</param>
    /// <param name="model">The model containing replacement data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the rendered HTML or plain text string.</returns>
    Task<string> RenderAsync(string template, object model, CancellationToken cancellationToken = default);
}
