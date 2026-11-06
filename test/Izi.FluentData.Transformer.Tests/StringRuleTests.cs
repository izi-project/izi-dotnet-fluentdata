using System.Globalization;
using System.Text.RegularExpressions;

namespace Izi.FluentData.Transformer.Tests;

public class StringRuleTests
{
    [Fact]
    public async Task Trim_rules_remove_only_the_requested_edge_and_treat_null_as_empty()
    {
        Assert.Equal("hello", await TransformerRules.Trim().TransformAsync("  hello  "));
        Assert.Equal("hello  ", await TransformerRules.TrimStart().TransformAsync("  hello  "));
        Assert.Equal("  hello", await TransformerRules.TrimEnd().TransformAsync("  hello  "));
        Assert.Equal(string.Empty, await TransformerRules.Trim().TransformAsync(null!));
        Assert.Equal(string.Empty, await TransformerRules.TrimStart().TransformAsync(null!));
        Assert.Equal(string.Empty, await TransformerRules.TrimEnd().TransformAsync(null!));
    }

    [Fact]
    public async Task Case_rules_default_to_invariant_and_honor_a_supplied_culture()
    {
        Assert.Equal("I", await TransformerRules.ToUpper().TransformAsync("i"));
        Assert.Equal("İ", await TransformerRules.ToUpper(CultureInfo.GetCultureInfo("tr-TR")).TransformAsync("i"));
        Assert.Equal("i", await TransformerRules.ToLower().TransformAsync("I"));
        Assert.Equal("ı", await TransformerRules.ToLower(CultureInfo.GetCultureInfo("tr-TR")).TransformAsync("I"));
        Assert.Equal("Hello World", await TransformerRules.ToTitleCase().TransformAsync("hello world"));
        Assert.Equal("ABC", await TransformerRules.ToTitleCase().TransformAsync("ABC"));
        Assert.Equal("Hello World", await TransformerRules.ToTitleCase(CultureInfo.GetCultureInfo("en-US")).TransformAsync("hello world"));
        Assert.Equal(string.Empty, await TransformerRules.ToUpper().TransformAsync(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToUpper(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToLower(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToTitleCase(null!));
    }

    [Fact]
    public async Task Replace_and_regex_rules_substitute_text()
    {
        Assert.Equal("h-llo", await TransformerRules.Replace("e", "-").TransformAsync("hello"));
        Assert.Equal(string.Empty, await TransformerRules.Replace("e", "-").TransformAsync(null!));
        Assert.Equal("h-llo", await TransformerRules.ReplaceRegex(new Regex("e+"), "-").TransformAsync("hello"));
        Assert.Equal("h-llo", await TransformerRules.ReplaceRegex("e+", "-").TransformAsync("hello"));
        Assert.Equal("h-llo", await TransformerRules.ReplaceRegex("E+", "-", RegexOptions.IgnoreCase).TransformAsync("hello"));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Replace(null!, "x"));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.Replace("x", null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ReplaceRegex((Regex)null!, "x"));
        Assert.Throws<RegexParseException>(() => TransformerRules.ReplaceRegex("[", "x"));
    }

    [Fact]
    public void A_caller_supplied_regex_keeps_its_own_timeout()
    {
        var regex = new Regex("(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(1));
        var rule = TransformerRules.ReplaceRegex(regex, "x");

        Assert.Throws<RegexMatchTimeoutException>(() => rule.TransformAsync(new string('a', 28) + "!"));
    }

    [Fact]
    public async Task Whitespace_rules_do_not_trim_when_they_collapse()
    {
        Assert.Equal("ab", await TransformerRules.RemoveWhitespace().TransformAsync(" a\tb "));
        Assert.Equal(" a b ", await TransformerRules.CollapseWhitespace().TransformAsync("  a \t b\n"));
        Assert.Equal(string.Empty, await TransformerRules.RemoveWhitespace().TransformAsync(null!));
    }

    [Fact]
    public async Task Slice_pad_and_affix_rules_validate_configuration_and_coalesce_null()
    {
        Assert.Equal("llo", await TransformerRules.Substring(2).TransformAsync("hello"));
        Assert.Equal("el", await TransformerRules.Substring(1, 2).TransformAsync("hello"));
        Assert.Equal("he", await TransformerRules.Truncate(2).TransformAsync("hello"));
        Assert.Equal(string.Empty, await TransformerRules.Truncate(2).TransformAsync(null!));
        Assert.Equal("**hi", await TransformerRules.PadLeft(4, '*').TransformAsync("hi"));
        Assert.Equal("hi**", await TransformerRules.PadRight(4, '*').TransformAsync("hi"));
        Assert.Equal("prehi", await TransformerRules.Prepend("pre").TransformAsync("hi"));
        Assert.Equal("hi", await TransformerRules.Prepend(null!).TransformAsync("hi"));
        Assert.Equal("post", await TransformerRules.Append("post").TransformAsync(null!));
        Assert.Equal("hipost", await TransformerRules.Append("post").TransformAsync("hi"));

        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Substring(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Substring(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Truncate(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.PadLeft(-1, ' '));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.PadRight(-1, ' '));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Substring(1).TransformAsync(null!));
    }

    [Fact]
    public async Task A_string_rule_chains_with_Then_and_observes_cancellation()
    {
        var pipeline = new Transformer<string, string>(TransformerRules.Trim())
            .Then(TransformerRules.ToUpper());
        Assert.Equal("HI", await pipeline.TransformAsync("  hi  "));

        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        Assert.Throws<OperationCanceledException>(() => TransformerRules.Trim().TransformAsync(" hi ", cancelled.Token));
    }
}
