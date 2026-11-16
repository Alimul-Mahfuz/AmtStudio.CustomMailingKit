using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using AmtStudio.CustomMailingKit.Abstractions;

namespace AmtStudio.CustomMailingKit.Services;

/// <summary>
/// A fast, lightweight template engine that replaces {{ Key }} placeholders from an object or dictionary.
/// </summary>
public sealed partial class SimpleEmailTemplateEngine : IEmailTemplateEngine
{
    [GeneratedRegex(@"\{\{\s*([a-zA-Z0-9_\.]+)\s*\}\}", RegexOptions.Compiled)]
    private static partial Regex TokenPattern();

    public string Render(string template, object model)
    {
        if (string.IsNullOrEmpty(template) || model is null)
        {
            return template ?? string.Empty;
        }

        var values = ExtractValues(model);

        return TokenPattern().Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            if (values.TryGetValue(key, out var val))
            {
                return val?.ToString() ?? string.Empty;
            }

            // If not found in dictionary, leave unchanged or return empty depending on design.
            // Leaving unchanged preserves unmatched tags or user can see what's missing.
            return match.Value;
        });
    }

    public Task<string> RenderAsync(string template, object model, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Render(template, model));
    }

    private static Dictionary<string, object?> ExtractValues(object model)
    {
        var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (model is IDictionary idict)
        {
            foreach (DictionaryEntry entry in idict)
            {
                if (entry.Key is not null)
                {
                    dict[entry.Key.ToString()!] = entry.Value;
                }
            }
            return dict;
        }

        var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (prop.CanRead)
            {
                dict[prop.Name] = prop.GetValue(model);
            }
        }

        return dict;
    }
}
