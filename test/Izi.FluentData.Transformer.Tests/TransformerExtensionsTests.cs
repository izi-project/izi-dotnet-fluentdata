namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Exercises the fluent <c>TransformerExtensions</c> against a freestanding <see cref="Transformer{T}"/>: each
/// method appends the matching built-in rule and returns the transformer, so chained steps compose and run in
/// order. Covers the string, numeric, date/time, and default-value families.
/// </summary>
public class TransformerExtensionsTests
{
    [Fact]
    public async Task String_pipeline_composes_in_order()
    {
        var result = await new Transformer<string>()
            .Trim()
            .ToUpper()
            .Truncate(3)
            .TransformAsync("  abcdef  ");

        Assert.Equal("ABC", result);
    }

    [Fact]
    public async Task String_pipeline_prepend_then_append()
    {
        var result = await new Transformer<string>()
            .Trim()
            .Prepend("[")
            .Append("]")
            .TransformAsync("  x  ");

        Assert.Equal("[x]", result);
    }

    [Fact]
    public async Task String_pipeline_replace_regex_then_collapse_whitespace()
    {
        var result = await new Transformer<string>()
            .ReplaceRegex(@"\d+", " ")
            .CollapseWhitespace()
            .Trim()
            .TransformAsync("a12b34c");

        Assert.Equal("a b c", result);
    }

    [Fact]
    public async Task String_pipeline_remove_whitespace()
    {
        var result = await new Transformer<string>()
            .RemoveWhitespace()
            .TransformAsync("  a b\tc\n ");

        Assert.Equal("abc", result);
    }

    [Fact]
    public async Task Numeric_pipeline_rounds_then_clamps()
    {
        var result = await new Transformer<decimal>()
            .Round(2)
            .Clamp(0m, 10m)
            .TransformAsync(12.345m);

        Assert.Equal(10m, result);
    }

    [Fact]
    public async Task Numeric_pipeline_abs_then_add()
    {
        var result = await new Transformer<int>()
            .Abs()
            .Add(1)
            .TransformAsync(-5);

        Assert.Equal(6, result);
    }

    [Fact]
    public async Task DateTime_pipeline_start_of_month_then_add_days()
    {
        var result = await new Transformer<DateTime>()
            .StartOfMonth()
            .AddDays(9)
            .TransformAsync(new DateTime(2024, 3, 25, 14, 0, 0));

        Assert.Equal(new DateTime(2024, 3, 10), result);
    }

    [Fact]
    public async Task Default_pipeline_falls_back_then_trims()
    {
        var result = await new Transformer<string>()
            .DefaultIfNull("  fallback  ")
            .Trim()
            .TransformAsync(null!);

        Assert.Equal("fallback", result);
    }
}
