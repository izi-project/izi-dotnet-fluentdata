using static Izi.FluentData.Transformer.Rules.Rules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the generic-math numeric rules across multiple numeric types: arithmetic, absolute value,
/// sign/invert, clamp, normalisation, and the rounding family (including banker's rounding defaults and
/// truncate/ceiling/floor).
/// </summary>
public class NumericRuleTests
{
    // ---- Arithmetic (generic math) ----
    [Fact]
    public async Task Add_offsets_value()
        => Assert.Equal(7, await Add(2).TransformAsync(5));

    [Fact]
    public async Task Subtract_offsets_value()
        => Assert.Equal(3, await Subtract(2).TransformAsync(5));

    [Fact]
    public async Task Multiply_scales_value()
        => Assert.Equal(10, await Multiply(2).TransformAsync(5));

    [Fact]
    public async Task Divide_scales_value()
        => Assert.Equal(5, await Divide(2).TransformAsync(10));

    // ---- Abs (generic math) ----
    [Fact]
    public async Task Abs_int()
        => Assert.Equal(5, await Abs<int>().TransformAsync(-5));

    [Fact]
    public async Task Abs_decimal()
        => Assert.Equal(2.5m, await Abs<decimal>().TransformAsync(-2.5m));

    [Fact]
    public async Task Abs_double()
        => Assert.Equal(3.0, await Abs<double>().TransformAsync(-3.0), 5);

    // ---- Sign / Invert ----
    [Theory]
    [InlineData(-5, -1)]
    [InlineData(0, 0)]
    [InlineData(42, 1)]
    public async Task Sign_maps_to_minus_one_zero_one(int input, int expected)
        => Assert.Equal(expected, await Sign<int>().TransformAsync(input));

    [Fact]
    public async Task Invert_negates()
        => Assert.Equal(-5, await Invert<int>().TransformAsync(5));

    // ---- Clamp (generic math) ----
    [Theory]
    [InlineData(15, 10)]   // above max
    [InlineData(-3, 0)]    // below min
    [InlineData(5, 5)]     // within range
    public async Task Clamp_int(int input, int expected)
        => Assert.Equal(expected, await Clamp(0, 10).TransformAsync(input));

    [Fact]
    public async Task Clamp_decimal()
        => Assert.Equal(1000m, await Clamp(0m, 1000m).TransformAsync(5000m));

    // ---- Normalize ----
    [Fact]
    public async Task Normalize_maps_into_unit_range()
        => Assert.Equal(0.5, await Normalize(0.0, 10.0).TransformAsync(5.0), 5);

    // ---- Round ----
    [Fact]
    public async Task Round_defaults_to_banker_rounding_zero_digits()
    {
        Assert.Equal(2m, await Round<decimal>().TransformAsync(2.5m)); // ToEven -> 2
        Assert.Equal(4m, await Round<decimal>().TransformAsync(3.5m)); // ToEven -> 4
    }

    [Fact]
    public async Task Round_with_digits_and_away_from_zero()
        => Assert.Equal(2.35m, await Round<decimal>(2, MidpointRounding.AwayFromZero).TransformAsync(2.345m));

    // ---- Truncate / Ceiling / Floor ----
    // (decimal literals can't appear in InlineData, so these are explicit facts.)
    [Fact]
    public async Task Truncate_drops_fraction()
    {
        Assert.Equal(2m, await Truncate<decimal>().TransformAsync(2.9m));
        Assert.Equal(-2m, await Truncate<decimal>().TransformAsync(-2.9m));
    }

    [Fact]
    public async Task Ceiling_rounds_up()
    {
        Assert.Equal(3m, await Ceiling<decimal>().TransformAsync(2.1m));
        Assert.Equal(-2m, await Ceiling<decimal>().TransformAsync(-2.1m));
    }

    [Fact]
    public async Task Floor_rounds_down()
    {
        Assert.Equal(2m, await Floor<decimal>().TransformAsync(2.9m));
        Assert.Equal(-3m, await Floor<decimal>().TransformAsync(-2.1m));
    }
}
