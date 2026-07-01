using static Izi.FluentData.Transformer.Rules.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the conversion rules: default-substitution, invariant-culture <c>{Type}ToString</c> formatting,
/// strict string-to-primitive parses (which throw on invalid input), numeric-to-numeric rounding, and the
/// generic <c>Convert</c> escape hatch for lenient/custom conversions.
/// </summary>
public class ConverterRuleTests
{
    // ---- DefaultIfNull ----
    [Fact]
    public async Task DefaultIfNull_replaces_null_reference()
        => Assert.Equal("fallback", await DefaultIfNull("fallback").TransformAsync(null!));

    [Fact]
    public async Task DefaultIfNull_keeps_non_null()
        => Assert.Equal("value", await DefaultIfNull("fallback").TransformAsync("value"));

    // ---- {Type}ToString (invariant culture) ----
    [Fact]
    public async Task DecimalToString_formats_with_invariant_culture()
        => Assert.Equal("1234.5", await DecimalToString().TransformAsync(1234.5m));

    [Fact]
    public async Task Int32ToString_formats()
        => Assert.Equal("42", await Int32ToString().TransformAsync(42));

    // ---- String -> numeric (strict; throws on invalid) ----
    [Fact]
    public async Task StringToInt32_parses_with_surrounding_whitespace()
        => Assert.Equal(42, await StringToInt32().TransformAsync(" 42 "));

    [Fact]
    public async Task StringToInt32_throws_on_invalid()
        => await Assert.ThrowsAsync<FormatException>(async () => await StringToInt32().TransformAsync("abc"));

    [Fact]
    public async Task StringToInt64_parses()
        => Assert.Equal(9_000_000_000L, await StringToInt64().TransformAsync("9000000000"));

    [Fact]
    public async Task StringToDecimal_parses_thousands_and_decimals()
        => Assert.Equal(1234.56m, await StringToDecimal().TransformAsync("1,234.56"));

    [Fact]
    public async Task StringToDouble_parses()
        => Assert.Equal(3.14, await StringToDouble().TransformAsync("3.14"), 2);

    // ---- String -> bool ----
    [Theory]
    [InlineData("true", true)]
    [InlineData("False", false)]
    [InlineData("  true  ", true)] // Boolean.Parse trims surrounding whitespace
    public async Task StringToBoolean_parses(string input, bool expected)
        => Assert.Equal(expected, await StringToBoolean().TransformAsync(input));

    [Fact]
    public async Task StringToBoolean_throws_on_invalid()
        => await Assert.ThrowsAsync<FormatException>(async () => await StringToBoolean().TransformAsync("yes"));

    // ---- String -> DateTime ----
    [Fact]
    public async Task StringToDateTime_parses_invariant()
        => Assert.Equal(new DateTime(2024, 1, 15), await StringToDateTime().TransformAsync("2024-01-15"));

    [Fact]
    public async Task StringToDateTime_throws_on_invalid()
        => await Assert.ThrowsAsync<FormatException>(async () => await StringToDateTime().TransformAsync("not-a-date"));

    // ---- Numeric -> numeric ----
    [Fact]
    public async Task DoubleToInt32_rounds_to_nearest()
        => Assert.Equal(3, await DoubleToInt32().TransformAsync(2.6));

    // ---- Custom (lenient) conversion via the generic Convert ----
    [Fact]
    public async Task Convert_supports_a_lenient_parse_with_fallback()
    {
        // Fully qualified so the factory isn't confused with System.Convert.
        var rule = Izi.FluentData.Transformer.Rules.Rules.Convert<string, int>(
            (s, _) => new ValueTask<int>(int.TryParse(s, out var n) ? n : -1));

        Assert.Equal(-1, await rule.TransformAsync("not-a-number"));
        Assert.Equal(7, await rule.TransformAsync("7"));
    }
}
