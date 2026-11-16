using AmtStudio.CustomMailingKit.Services;
using Xunit;

namespace AmtStudio.CustomMailingKit.Tests;

public class TemplateEngineTests
{
    private readonly SimpleEmailTemplateEngine _engine = new();

    [Fact]
    public void Render_ReplacesTokens_FromAnonymousObject()
    {
        var template = "Hello {{Name}}, your order {{OrderId}} is ready!";
        var model = new { Name = "Alice", OrderId = 12345 };

        var result = _engine.Render(template, model);

        Assert.Equal("Hello Alice, your order 12345 is ready!", result);
    }

    [Fact]
    public void Render_HandlesSpacesInsideMustacheTags()
    {
        var template = "Welcome, {{  UserName   }} to {{ Organization }}!";
        var model = new { UserName = "Bob", Organization = "AmtStudio" };

        var result = _engine.Render(template, model);

        Assert.Equal("Welcome, Bob to AmtStudio!", result);
    }

    [Fact]
    public void Render_ReplacesTokens_FromDictionary()
    {
        var template = "Your verification code is {{Code}}.";
        var dict = new Dictionary<string, object>
        {
            ["Code"] = "987654"
        };

        var result = _engine.Render(template, dict);

        Assert.Equal("Your verification code is 987654.", result);
    }

    [Fact]
    public void Render_LeavesUnmatchedTagsUnchanged()
    {
        var template = "Hello {{Name}}, your code is {{Code}}.";
        var model = new { Name = "Charlie" };

        var result = _engine.Render(template, model);

        Assert.Equal("Hello Charlie, your code is {{Code}}.", result);
    }

    [Fact]
    public void Render_ReturnsEmptyOrOriginalWhenNull()
    {
        Assert.Equal("", _engine.Render("", new { }));
        Assert.Equal("Plain text", _engine.Render("Plain text", null!));
    }
}
