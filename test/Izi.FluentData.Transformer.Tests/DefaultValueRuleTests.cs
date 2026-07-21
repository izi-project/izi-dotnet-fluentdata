using static Izi.FluentData.Transformer.Rules.TransformerRules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the default-substitution rules: predicate-driven <c>DefaultIf</c>, the null/empty/whitespace shortcuts,
/// and the min/max rules in both their sentinel form (<c>T.MinValue</c>/<c>T.MaxValue</c>) and their comparable
/// threshold form.
/// </summary>
public class DefaultValueRuleTests
{
    [Fact]
    public async Task DefaultIf_substitutes_when_predicate_matches()
        => Assert.Equal(0, await DefaultIf(0, n => n < 0).TransformAsync(-5));

    [Fact]
    public async Task DefaultIf_keeps_value_when_predicate_fails()
        => Assert.Equal(5, await DefaultIf(0, n => n < 0).TransformAsync(5));

    [Fact]
    public async Task DefaultIfNull_replaces_null()
        => Assert.Equal("fallback", await DefaultIfNull("fallback").TransformAsync(null!));

    [Fact]
    public async Task DefaultIfNull_keeps_non_null()
        => Assert.Equal("value", await DefaultIfNull("fallback").TransformAsync("value"));

    [Theory]
    [InlineData("", "fallback")]
    [InlineData(null, "fallback")]
    [InlineData("value", "value")]
    public async Task DefaultIfEmpty_replaces_null_or_empty_string(string? input, string expected)
        => Assert.Equal(expected, await DefaultIfEmpty("fallback").TransformAsync(input!));

    [Fact]
    public async Task DefaultIfEmpty_replaces_empty_collection()
    {
        var fallback = new List<int> { 1 };
        Assert.Same(fallback, await DefaultIfEmpty(fallback).TransformAsync([]));
    }

    [Fact]
    public async Task DefaultIfEmpty_keeps_populated_collection()
    {
        var populated = new List<int> { 1, 2 };
        Assert.Same(populated, await DefaultIfEmpty(new List<int> { 9 }).TransformAsync(populated));
    }

    [Theory]
    [InlineData("", "fallback")]
    [InlineData("   ", "fallback")]
    [InlineData(null, "fallback")]
    [InlineData("value", "value")]
    public async Task DefaultIfNullOrWhitespace_replaces_blank(string? input, string expected)
        => Assert.Equal(expected, await DefaultIfNullOrWhitespace("fallback").TransformAsync(input!));

    // ---- Sentinel min/max ----
    [Fact]
    public async Task DefaultIfMinValue_replaces_type_min()
        => Assert.Equal(0, await DefaultIfMinValue(0).TransformAsync(int.MinValue));

    [Fact]
    public async Task DefaultIfMinValue_keeps_other_values()
        => Assert.Equal(5, await DefaultIfMinValue(0).TransformAsync(5));

    [Fact]
    public async Task DefaultIfMaxValue_replaces_type_max()
        => Assert.Equal(0, await DefaultIfMaxValue(0).TransformAsync(int.MaxValue));

    // ---- Comparable threshold form ----
    [Fact]
    public async Task DefaultIfMinValue_threshold_replaces_at_or_below()
    {
        var rule = DefaultIfMinValue(minimum: 10, defaultValue: -1);
        Assert.Equal(-1, await rule.TransformAsync(10));
        Assert.Equal(-1, await rule.TransformAsync(3));
        Assert.Equal(11, await rule.TransformAsync(11));
    }

    [Fact]
    public async Task DefaultIfMaxValue_threshold_replaces_at_or_above()
    {
        var rule = DefaultIfMaxValue(maximum: 100, defaultValue: -1);
        Assert.Equal(-1, await rule.TransformAsync(100));
        Assert.Equal(-1, await rule.TransformAsync(250));
        Assert.Equal(99, await rule.TransformAsync(99));
    }
}
