using Izi.FluentData.Transformer.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Exercises the fluent builder extension methods, verifying that chained steps compose correctly — including
/// type-changing chains (string → numeric → string), default substitution, decimal rounding, and the custom
/// converter escape hatch.
/// </summary>
public class RuleBuilderExtensionsTests
{
    /// <summary>Test helper that starts a fluent pipeline rooted at type <typeparamref name="T"/>.</summary>
    internal static class Pipeline
    {
        /// <summary>Creates an identity-seeded builder for type <typeparamref name="T"/>.</summary>
        public static TransformerRuleBuilder<T, T> For<T>() => new TransformerRuleBuilder<T>();
    }

    [Fact]
    public async Task String_normalisation_chain()
    {
        var rule = Pipeline.For<string>()
            .Trim()
            .ToUpper()
            .Truncate(3)
            .Build();

        Assert.Equal("ABC", await rule.TransformAsync("  abcdef  "));
    }

    [Fact]
    public async Task Convert_chain_changes_type_through_pipeline()
    {
        // string -> int -> clamp, all via the fluent builder
        var rule = Pipeline.For<string>()
            .Trim()
            .StringToInt32()
            .Clamp(0, 100)
            .Build();

        Assert.Equal(100, await rule.TransformAsync(" 150 "));
    }

    [Fact]
    public async Task Numeric_then_back_to_string()
    {
        var rule = Pipeline.For<int>()
            .Abs()
            .Int32ToString()
            .Append("!")
            .Build();

        Assert.Equal("5!", await rule.TransformAsync(-5));
    }

    [Fact]
    public async Task DefaultIfNull_then_trim()
    {
        var rule = Pipeline.For<string>()
            .DefaultIfNull("fallback")
            .Trim()
            .Build();

        Assert.Equal("fallback", await rule.TransformAsync(null!));
    }

    [Fact]
    public async Task Decimal_rounding_chain()
    {
        var rule = Pipeline.For<decimal>()
            .Round(2, MidpointRounding.AwayFromZero)
            .Clamp(0m, 10m)
            .Build();

        Assert.Equal(10m, await rule.TransformAsync(12.345m));
        Assert.Equal(2.35m, await rule.TransformAsync(2.345m));
    }

    [Fact]
    public async Task Custom_converter_falls_back()
    {
        var rule = Pipeline.For<string>()
            .Convert((s, _) => new ValueTask<int>(int.TryParse(s, out var n) ? n : -1))
            .Build();

        Assert.Equal(-1, await rule.TransformAsync("not-a-number"));
    }
}
