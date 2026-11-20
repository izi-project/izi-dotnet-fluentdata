using System.Globalization;
using System.Text.RegularExpressions;
using Izi.FluentData.Transformer.Exceptions;

namespace Izi.FluentData.Transformer.Tests;

public class StringRuleTests
{
    [Fact]
    public async Task Trim_rules_remove_only_the_requested_edge_and_keep_null()
    {
        Assert.Equal("hello", await TransformerRules.Trim().TransformAsync("  hello  "));
        Assert.Equal("hello  ", await TransformerRules.TrimStart().TransformAsync("  hello  "));
        Assert.Equal("  hello", await TransformerRules.TrimEnd().TransformAsync("  hello  "));
        Assert.Null(await TransformerRules.Trim().TransformAsync(null!));
        Assert.Null(await TransformerRules.TrimStart().TransformAsync(null!));
        Assert.Null(await TransformerRules.TrimEnd().TransformAsync(null!));
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
        Assert.Equal(
            "Hello World",
            await TransformerRules.ToTitleCase(CultureInfo.GetCultureInfo("en-US")).TransformAsync("hello world"));
        Assert.Null(await TransformerRules.ToUpper().TransformAsync(null!));
        Assert.Null(await TransformerRules.ToLower().TransformAsync(null!));
        Assert.Null(await TransformerRules.ToTitleCase().TransformAsync(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToUpper(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToLower(null!));
        Assert.Throws<ArgumentNullException>(() => TransformerRules.ToTitleCase(null!));
    }

    [Fact]
    public async Task Replace_and_regex_rules_substitute_text()
    {
        Assert.Equal("h-llo", await TransformerRules.Replace("e", "-").TransformAsync("hello"));
        Assert.Null(await TransformerRules.Replace("e", "-").TransformAsync(null!));
        Assert.Equal("h-llo", await TransformerRules.ReplaceRegex(new Regex("e+", RegexOptions.None, TimeSpan.FromSeconds(1)), "-").TransformAsync("hello"));
        Assert.Equal("h-llo", await TransformerRules.ReplaceRegex("e+", "-").TransformAsync("hello"));
        Assert.Equal(
            "h-llo",
            await TransformerRules.ReplaceRegex("E+", "-", RegexOptions.IgnoreCase).TransformAsync("hello"));
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

        var timeout = Assert.Throws<TransformationRuleException>(() => rule.TransformAsync(new string('a', 28) + "!"));
        Assert.IsType<RegexMatchTimeoutException>(timeout.InnerException);
    }

    [Fact]
    public async Task Whitespace_rules_do_not_trim_when_they_collapse()
    {
        Assert.Equal("ab", await TransformerRules.RemoveWhitespace().TransformAsync(" a\tb "));
        Assert.Equal(" a b ", await TransformerRules.CollapseWhitespace().TransformAsync("  a \t b\n"));
        Assert.Null(await TransformerRules.RemoveWhitespace().TransformAsync(null!));
        Assert.Null(await TransformerRules.CollapseWhitespace().TransformAsync(null!));
    }

    [Fact]
    public async Task Slice_pad_and_affix_rules_validate_configuration_and_keep_null()
    {
        Assert.Equal("llo", await TransformerRules.Substring(2).TransformAsync("hello"));
        Assert.Equal("el", await TransformerRules.Substring(1, 2).TransformAsync("hello"));
        Assert.Equal("he", await TransformerRules.Truncate(2).TransformAsync("hello"));
        Assert.Null(await TransformerRules.Truncate(2).TransformAsync(null!));
        Assert.Equal("**hi", await TransformerRules.PadLeft(4, '*').TransformAsync("hi"));
        Assert.Equal("hi**", await TransformerRules.PadRight(4, '*').TransformAsync("hi"));
        Assert.Equal("prehi", await TransformerRules.Prepend("pre").TransformAsync("hi"));
        Assert.Equal("hi", await TransformerRules.Prepend(null!).TransformAsync("hi"));
        Assert.Null(await TransformerRules.Append("post").TransformAsync(null!));
        Assert.Equal("hipost", await TransformerRules.Append("post").TransformAsync("hi"));

        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Substring(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Substring(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.Truncate(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.PadLeft(-1, ' '));
        Assert.Throws<ArgumentOutOfRangeException>(() => TransformerRules.PadRight(-1, ' '));
        Assert.Null(await TransformerRules.Substring(1).TransformAsync(null!));
        Assert.Null(await TransformerRules.Substring(1, 2).TransformAsync(null!));
        Assert.Null(await TransformerRules.PadLeft(4, '*').TransformAsync(null!));
        Assert.Null(await TransformerRules.PadRight(4, '*').TransformAsync(null!));
        Assert.Null(await TransformerRules.Prepend("pre").TransformAsync(null!));
        Assert.Null(await TransformerRules.Append(null).TransformAsync(null!));
        var timed = new Regex("e+", RegexOptions.None, TimeSpan.FromSeconds(1));
        Assert.Null(await TransformerRules.ReplaceRegex(timed, "-").TransformAsync(null!));
        Assert.Null(await TransformerRules.ReplaceRegex("e+", "-").TransformAsync(null!));
        Assert.Null(await TransformerRules.ReplaceRegex("E+", "-", RegexOptions.IgnoreCase).TransformAsync(null!));
    }

    [Fact]
    public async Task Trim_then_upper_and_cancellation_are_observed_on_the_rules()
    {
        var trimmed = await TransformerRules.Trim().TransformAsync("  hi  ");
        Assert.Equal("HI", await TransformerRules.ToUpper().TransformAsync(trimmed));

        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        Assert.Throws<OperationCanceledException>(() =>
            TransformerRules.Trim().TransformAsync(" hi ", cancelled.Token));
    }
}
