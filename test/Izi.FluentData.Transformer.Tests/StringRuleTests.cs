using System.Globalization;
using static Izi.FluentData.Transformer.Rules.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the string rules: trimming, casing (with explicit and invariant cultures), replace, substring,
/// truncate, padding, prepend/append, and the blank/empty default-substitution rules — including their
/// null-coalescing behaviour.
/// </summary>
public class StringRuleTests
{
    [Theory]
    [InlineData("  hello  ", "hello")]
    [InlineData("nospace", "nospace")]
    [InlineData("\t tabbed \n", "tabbed")]
    public async Task Trim_removes_surrounding_whitespace(string input, string expected)
        => Assert.Equal(expected, await Trim().TransformAsync(input));

    [Fact]
    public async Task Trim_coalesces_null_to_empty()
        => Assert.Equal(string.Empty, await Trim().TransformAsync(null!));

    [Fact]
    public async Task TrimStart_only_trims_leading()
        => Assert.Equal("x  ", await TrimStart().TransformAsync("  x  "));

    [Fact]
    public async Task TrimEnd_only_trims_trailing()
        => Assert.Equal("  x", await TrimEnd().TransformAsync("  x  "));

    [Fact]
    public async Task ToUpper_uses_invariant_culture_by_default()
        => Assert.Equal("ABC", await ToUpper().TransformAsync("abc"));

    [Fact]
    public async Task ToUpper_honours_supplied_culture()
        => Assert.Equal("ABC", await ToUpper(CultureInfo.GetCultureInfo("en-US")).TransformAsync("abc"));

    [Fact]
    public async Task ToLower_lowers()
        => Assert.Equal("abc", await ToLower().TransformAsync("ABC"));

    [Fact]
    public async Task ToTitleCase_capitalises_each_word()
        => Assert.Equal("Hello World", await ToTitleCase().TransformAsync("hello world"));

    [Fact]
    public async Task ToUpper_null_is_empty()
        => Assert.Equal(string.Empty, await ToUpper().TransformAsync(null!));

    [Fact]
    public async Task Replace_swaps_all_occurrences()
        => Assert.Equal("a_b_c", await Replace("-", "_").TransformAsync("a-b-c"));

    [Theory]
    [InlineData("hello", 1, "ello")]
    [InlineData("hello", 0, "hello")]
    public async Task Substring_from_start_index(string input, int start, string expected)
        => Assert.Equal(expected, await Substring(start).TransformAsync(input));

    [Theory]
    [InlineData("hello", 1, 2, "el")]
    [InlineData("hello", 0, 5, "hello")]
    public async Task Substring_with_length(string input, int start, int length, string expected)
        => Assert.Equal(expected, await Substring(start, length).TransformAsync(input));

    [Theory]
    [InlineData("hello", 3, "hel")]
    [InlineData("hi", 5, "hi")]
    [InlineData("hello", 0, "")]
    public async Task Truncate_caps_length(string input, int max, string expected)
        => Assert.Equal(expected, await Truncate(max).TransformAsync(input));

    [Fact]
    public async Task Truncate_null_is_empty()
        => Assert.Equal(string.Empty, await Truncate(3).TransformAsync(null!));

    [Fact]
    public async Task PadLeft_pads_with_char()
        => Assert.Equal("005", await PadLeft(3, '0').TransformAsync("5"));

    [Fact]
    public async Task PadLeft_null_pads_from_empty()
        => Assert.Equal("000", await PadLeft(3, '0').TransformAsync(null!));

    [Fact]
    public async Task PadRight_pads_with_char()
        => Assert.Equal("5..", await PadRight(3, '.').TransformAsync("5"));

    [Fact]
    public async Task Prepend_adds_prefix()
        => Assert.Equal("pre-x", await Prepend("pre-").TransformAsync("x"));

    [Fact]
    public async Task Append_adds_suffix()
        => Assert.Equal("x-post", await Append("-post").TransformAsync("x"));

    [Theory]
    [InlineData("", "fallback")]
    [InlineData("   ", "fallback")]
    [InlineData(null, "fallback")]
    [InlineData("value", "value")]
    public async Task DefaultIfNullOrWhitespace_replaces_blank(string? input, string expected)
        => Assert.Equal(expected, await DefaultIfNullOrWhitespace("fallback").TransformAsync(input!));

    [Theory]
    [InlineData("", "fallback")]
    [InlineData(null, "fallback")]
    [InlineData("value", "value")]
    public async Task DefaultIfEmpty_replaces_null_or_empty(string? input, string expected)
        => Assert.Equal(expected, await DefaultIfEmpty("fallback").TransformAsync(input!));
}
