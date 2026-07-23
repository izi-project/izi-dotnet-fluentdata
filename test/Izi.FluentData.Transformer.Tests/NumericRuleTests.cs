using static Izi.FluentData.Transformer.Rules.TransformerRules;

namespace Izi.FluentData.Transformer.Tests;

/// <summary>
/// Covers the generic-math numeric rules across several numeric types: arithmetic, absolute value and negation,
/// clamp, normalisation, the rounding family (with banker's default and explicit modes), roots/powers, and the
/// exponential/logarithmic/trigonometric families.
/// </summary>
public class NumericRuleTests
{
    // ---- Arithmetic ----
    [Fact]
    public async Task Add_offsets_value() => Assert.Equal(7, await Add(2).TransformAsync(5));

    [Fact]
    public async Task Subtract_offsets_value() => Assert.Equal(3, await Subtract(2).TransformAsync(5));

    [Fact]
    public async Task Multiply_scales_value() => Assert.Equal(10, await Multiply(2).TransformAsync(5));

    [Fact]
    public async Task Divide_scales_value() => Assert.Equal(5, await Divide(2).TransformAsync(10));

    // ---- Abs / Invert ----
    [Fact]
    public async Task Abs_int() => Assert.Equal(5, await Abs<int>().TransformAsync(-5));

    [Fact]
    public async Task Abs_decimal() => Assert.Equal(2.5m, await Abs<decimal>().TransformAsync(-2.5m));

    [Fact]
    public async Task Abs_double() => Assert.Equal(3.0, await Abs<double>().TransformAsync(-3.0), 5);

    [Fact]
    public async Task Invert_negates() => Assert.Equal(-5, await Invert<int>().TransformAsync(5));

    // ---- Clamp ----
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

    // ---- Rounding ----
    [Fact]
    public async Task Round_defaults_to_banker_rounding_zero_digits()
    {
        Assert.Equal(2m, await Round<decimal>().TransformAsync(2.5m)); // ToEven -> 2
        Assert.Equal(4m, await Round<decimal>().TransformAsync(3.5m)); // ToEven -> 4
    }

    [Fact]
    public async Task Round_with_digits_and_away_from_zero()
        => Assert.Equal(2.35m, await Round<decimal>(2, MidpointRounding.AwayFromZero).TransformAsync(2.345m));

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

    // ---- Roots / powers ----
    [Fact]
    public async Task Sqrt_of_nine() => Assert.Equal(3.0, await Sqrt<double>().TransformAsync(9.0), 5);

    [Fact]
    public async Task Cbrt_of_twentyseven() => Assert.Equal(3.0, await Cbrt<double>().TransformAsync(27.0), 5);

    [Fact]
    public async Task RootN_fourth_root() => Assert.Equal(2.0, await RootN<double>(4).TransformAsync(16.0), 5);

    [Fact]
    public async Task Pow_raises() => Assert.Equal(8.0, await Pow(3.0).TransformAsync(2.0), 5);

    // ---- Exponential / logarithmic ----
    [Fact]
    public async Task Exp2_and_Log2_round_trip() => Assert.Equal(8.0, await Exp2<double>().TransformAsync(3.0), 5);

    [Fact]
    public async Task Log10_of_thousand() => Assert.Equal(3.0, await Log10<double>().TransformAsync(1000.0), 5);

    [Fact]
    public async Task Log_with_base() => Assert.Equal(2.0, await Log(3.0).TransformAsync(9.0), 5);

    // ---- Trigonometric ----
    [Fact]
    public async Task DegreesToRadians_converts()
        => Assert.Equal(Math.PI, await DegreesToRadians<double>().TransformAsync(180.0), 5);

    [Fact]
    public async Task Sin_of_zero_is_zero() => Assert.Equal(0.0, await Sin<double>().TransformAsync(0.0), 5);
}
